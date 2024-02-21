import time
from math import cos as cos
from math import sin as sin
import math
from filterpy.kalman import MerweScaledSigmaPoints
from filterpy.kalman import UnscentedKalmanFilter as UKF
from filterpy.common import Q_discrete_white_noise
import matplotlib.pyplot as plt
import numpy as np
import pandas as pd

dT = 0.1  # 10 hz => 100ms

SIGMA_ALPHA_VALUE = 0.2
SIGMA_BETA_VALUE = 2.0
SIGMA_KAPPA_VALUE = 1.0

# Q_VARIANCE_VALUE = 0.1

# file_path = 'con_vel_basic_log.csv'  # alpha = 0.2
# file_path = 'con_vel_beacon_freq_log.csv'  # alpha = 0.2
file_path = 'con_vel_beacon_flag_AND_freq_log.csv'  # alpha = 0.2

# file_path = 'dyn_acc_basic_log.csv'   # alpha = 0.2
# file_path = 'dyn_acc_beacon_freq_log.csv'   # alpha = 0.2
# file_path = 'dyn_acc_beacon_flag_AND_freq_log.csv'  # alpha = 0.2

df_head = pd.read_csv(file_path, sep='\t', header=None, nrows=1, names=[

    'Zeit_Index',
    'Beacon0_Pos_X', 'Beacon0_Pos_Y',
    'Beacon1_Pos_X', 'Beacon1_Pos_Y',
    'Beacon2_Pos_X', 'Beacon2_Pos_Y',
    'GT_StartPos_X_Boat', 'GT_StartPos_Y_Boat'

])


# Erst ab 2 Zeile lesen
df = pd.read_csv(file_path, sep='\t', header=None, skiprows=1, names=[

    'Zeit_Index',
    'X_GT', 'Y_GT',
    'Heading_GT', 'Heading', 'Heading_STD',
    'angle_GT_B0', 'angleDistorted_B0', 'STD_angle_B0',
    'angle_GT_B1', 'angleDistorted_B1', 'STD_angle_B1',
    'angle_GT_B2', 'angleDistorted_B2', 'STD_angle_B2',
    'Richtung_GT_X_B0', 'Richtung_GT_Y_B0',
    'Richtung_X_B0', 'Richtung_Y_B0', 'STD_Richtung_B0',
    'Richtung_GT_X_B1', 'Richtung_GT_Y_B1',
    'Richtung_X_B1', 'Richtung_Y_B1', 'STD_Richtung_B1',
    'Richtung_GT_X_B2', 'Richtung_GT_Y_B2',
    'Richtung_X_B2', 'Richtung_Y_B2', 'STD_Richtung_B2',
    'Entfernung_B0', 'EntfernungDistorted_B0', 'STD_Entfernung_B0',
    'Entfernung_B1', 'EntfernungDistorted_B1', 'STD_Entfernung_B1',
    'Entfernung_B2', 'EntfernungDistorted_B2', 'STD_Entfernung_B2'

])


"""
UKF mit Winkel.
"""

# Imports


# --- ---- --- --- ---- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---

# Konfigurationsvariablen

BEACON_POSITIONS = [
    np.array([df_head['Beacon0_Pos_X'].values[0],
             df_head['Beacon0_Pos_Y'].values[0]]),
    np.array([df_head['Beacon1_Pos_X'].values[0],
             df_head['Beacon1_Pos_Y'].values[0]]),
    np.array([df_head['Beacon2_Pos_X'].values[0],
             df_head['Beacon2_Pos_Y'].values[0]])
]


# --- ---- --- --- ---- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---


def get_RMSE_from_Q(Q_variance):

    Q_VARIANCE_VALUE = Q_variance

    def f_x(state, dt):
        """
        Zustandsübergangsfunktion.
        Da die Geschwindigkeit konstant ist und der Zustand nur die Position beinhaltet,
        hat diese Funktion einfach die Geschwindigkeit zur Position hinzugefügt.
        """

        x, y, vx, vy, ax, ay = state

        # Update position with current velocity and acceleration
        new_x = x + vx * dt + (ax * dt ** 2) / 2.0
        new_y = y + vy * dt + (ay * dt ** 2) / 2.0

        # Update velocity with current acceleration
        new_vx = vx + ax * dt
        new_vy = vy + ay * dt

        # Speed stays constant (no acceleration yet)
        new_ax = ax
        new_ay = ay

        return np.array([new_x, new_y, new_vx, new_vy, new_ax, new_ay])

    def normalize_angle_deg(angle):
        """Normalisiert Winkel in Grad (so dass er im Bereich von -180bis 180 Grad liegt)"""

        # Normalisierung des Winkels in Grad (in Bereich von -180 bis 180 Grad)
        # angle = (angle + 180) % 360 - 180

        # angle_rad = deg_to_rad(angle)
        # rad = normalize_angle_rad(angle_rad)

        # return rad_to_degree(rad)

        # Winkel in BEreich von 0 bis 2pi (360) halten
        angle = angle % (2 * 180)

        # Wenn Winkel größer als pi (180) ist -> Anpassung um diesen in -180 bis 180 zu bringen
        if angle > 180:
            angle -= 2 * 180

        # Rückgabe des normalisierten Winkels (aber in Grad)
        # return x * 180 / np.pi
        return angle

    def residual_angle(a, b):
        """ 
        Berechnung der Differenz zwischen den zwo Winkeln (zwischen dem geschätzten und gemessenen Winkel)
        Quelle: https://github.com/rlabbe/Kalman-and-Bayesian-Filters-in-Python/blob/master/10-Unscented-Kalman-Filter.ipynb
        """
        # Berechnung der Differenz
        diff = a - b

        # Normalisierung der errechneten Winkeldifferenz (für Bereich von -180 bis 180 Grad)

        for i in range(len(diff)):
            diff[i] = normalize_angle_deg(diff[i])

        return diff

    def normalize_angle_rad(x):
        """ Normalisiert einen Winkel (in Radiant) für Bereich -pi (-180) bis pi (180)."""

        # Winkel in BEreich von 0 bis 2pi (360) halten
        x = x % (2 * np.pi)

        # Wenn Winkel größer als pi (180) ist -> Anpassung um diesen in -180 bis 180 zu bringen
        if x > np.pi:
            x -= 2 * np.pi

        # Rückgabe des normalisierten Winkels (aber in Grad)
        # return x * 180 / np.pi
        return x

    def rad_to_degree(x):
        return x * 180 / np.pi

    def deg_to_rad(x):
        return x * np.pi / 180

    def h_x(state):
        """
        Messfunktion.
        """

        # Position, Geschwindigkeit des Schiffs aus dem Zustand
        x, y, vx, vy, _, _ = state

        # Messliste
        measure_data = []

        # Richtungsvektor der Schiffsausrichtung
        hdg = np.array([vx, vy])

        # Berechnung der Magnitude (Geschwindigkeit)
        mag = np.sqrt(vx * vx + vy * vy)

        # Vermeidung Division durch 0 (der Geschwindigkeit, bspw. initial Zustand)
        if mag == 0:
            mag = 1

        # Normalisierung der Richtung
        hdg = np.array([vx / mag, vy / mag])

        # Rotation um 90 Grad zur Ausrichtung
        hdg = np.array([hdg[1], -hdg[0]])

        # Berechnung des Winkels zur Ausrichtung: y, x
        angleHeading = np.arctan2(hdg[1], hdg[0])

        for beacon_pos in BEACON_POSITIONS:

            # Berechnung der euklidischen Distanz
            # distance = np.sqrt((beacon_pos[0] - x) ** 2 + (beacon_pos[1] - y) ** 2)

            # Berechnung des Winkels (in Radiant) -- aktuellen Position und dem Beacon
            angle = np.arctan2(beacon_pos[1] - y, beacon_pos[0] - x)

            # Anpassung basierend zur Ausrichtung des Schiffs
            angle -= angleHeading

            # Normalisierung des Winkels
            normalized_angle = rad_to_degree(normalize_angle_rad(angle))

            # Speichern des normalisierten Winkels (für jeden Beacon)
            measure_data.append(normalized_angle)

        # Rückgabe der Winkel (Array)
        return np.array(measure_data)

    def sigma_subtract(a, b):
        # print(f"A: {a} \t B: {b}")
        diff = a - b
        for i in range(len(diff)):
            diff[i] = normalize_angle_deg(diff[i])
        return diff

    def init_ukf():
        """
        Initialisierung vom UKF.
        """

        # sigmas = MerweScaledSigmaPoints(
        # n=6, alpha=SIGMA_ALPHA_VALUE, beta=SIGMA_BETA_VALUE, kappa=SIGMA_KAPPA_VALUE)
        sigmas = MerweScaledSigmaPoints(
            n=6, alpha=SIGMA_ALPHA_VALUE, beta=SIGMA_BETA_VALUE, kappa=SIGMA_KAPPA_VALUE, subtract=sigma_subtract)

        # Dimension z ist 3 (da wir drei verzerrte Winkel als Messungen haben)
        ukf = UKF(dim_x=6, dim_z=3, dt=dT, fx=f_x, hx=h_x, points=sigmas,
                  residual_x=np.subtract, residual_z=residual_angle)

        # Initialzustand aus dem ersten Logeintrag holen
        init_state = np.array([
            df['X_GT'].values[0], df['Y_GT'].values[0],  # x, y
            0, 0,  # vx, vy
            0, 0   # ax, ay
        ])

        ukf.x = init_state

        ukf.Q = Q_discrete_white_noise(
            dim=2, dt=dT, var=Q_VARIANCE_VALUE, block_size=3, order_by_dim=False)

        return ukf

    def calc_rmse(uxs, gt):
        """
        Berechne den Root-Square-Mean-Error zwischen den UKF-Schätzungen und dem GT.
        """

        if len(uxs) == 0:
            raise ValueError("Liste der Schätzung ist leer!")

        if len(uxs) != len(gt):
            raise ValueError("Schätzungen sind nicht gleich so lang wie GT.")

        # Position, erste beiden Spalten
        uxs_posi = uxs[:, :2]

        rmse = np.sqrt(((uxs_posi - gt) ** 2).mean(axis=0))

        return rmse

    # MAIN
    # Speichere geschätzten Zustände und Kovarianzmatrizen
    uxs = []
    uPs = []

    # Init UKF.
    ukf = init_ukf()

    # Initial Zustand fix im Code vs. aus dem ersten Log
    # ukf.x = initial_state  #  passiert bereits in init_ukf-Funktion

    # Iteriere durch jede Zeile im Df
    for index, row in df.iterrows():

        # Standardabweichungen für die aktuelle Messung aus Spalte holen
        # Wichtig: Nochmal Division-Faktor anschauen
        std_b0 = row['STD_angle_B0']
        std_b1 = row['STD_angle_B1']
        std_b2 = row['STD_angle_B2']

        # Zeige Std zwecks Korrektheit
        # print(std_b0, std_b1, std_b2)

        # Setze die Rauschmatrix R basierend auf den extrahierten Standardabweichungen immer neu
        ukf.R = np.diag([std_b0 ** 2, std_b1 ** 2, std_b2 ** 2])

        # Prüfe ob Std. in R-Matrix gesetzt werden
        # print(ukf.R)

        # Hole den aktuellen Messvektor z
        z = np.array([
            row['angleDistorted_B0'],
            row['angleDistorted_B1'],
            row['angleDistorted_B2'],
        ])

        # Check ob Daten korrekt eingelesen sind
        # print(z)

        # Vorhersage und Update mit dem aktuellen Messvektor
        ukf.predict()
        ukf.update(z)

        uxs.append(ukf.x.copy())
        uPs.append(ukf.P.copy())

    # Gefilterte Zustände [x, y, vx, vy, ax, ay]
    uxs = np.array(uxs)

    ## Auswertungsfunktionen ##
    # ==> RMSE für Strecke

    gt = df[['X_GT', 'Y_GT']].values

    rmse = calc_rmse(uxs, gt)
    return rmse


print("Q\tRMSE\tRMSE_x_y")

# 0.01 ... 0.1
for i in range(10):
    q = 0.01 * (i+1)
    try:
        rmse = get_RMSE_from_Q(q)
        sum_rmse = "%.2f" % (rmse[0] + rmse[1])  # round to two decimals
        print(f"{q}\t{(sum_rmse)}\t{rmse}")
    except:
        print(f"{q}\t--fail")

print()
# 0.1 ... 1
for i in range(10):
    q = 0.1 * (i+1)
    try:
        rmse = get_RMSE_from_Q(q)
        sum_rmse = "%.2f" % (rmse[0] + rmse[1])  # round to two decimals
        print(f"{q}\t{(sum_rmse)}\t{rmse}")
    except:
        print(f"{q}\t--fail")

print()
# 1 ... 10
for i in range(10):
    q = 1 * (i+1)
    try:
        rmse = get_RMSE_from_Q(q)
        sum_rmse = "%.2f" % (rmse[0] + rmse[1])  # round to two decimals
        print(f"{q}\t{(sum_rmse)}\t{rmse}")
    except:
        print(f"{q}\t--fail")

print()
# 10 ... 100
for i in range(10):
    q = 10 * (i+1)
    try:
        rmse = get_RMSE_from_Q(q)
        sum_rmse = "%.2f" % (rmse[0] + rmse[1])  # round to two decimals
        print(f"{q}\t{(sum_rmse)}\t{rmse}")
    except:
        print(f"{q}\t--fail")
