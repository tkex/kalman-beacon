import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from filterpy.common import Q_discrete_white_noise
from filterpy.kalman import UnscentedKalmanFilter as UKF
from filterpy.kalman import MerweScaledSigmaPoints
import math
from math import sin as sin
from math import cos as cos
import time
import pandas as pd


dT = 0.1  # 10 hz => 100ms

SIGMA_ALPHA_VALUE = 0.2
SIGMA_BETA_VALUE = 2.0
SIGMA_KAPPA_VALUE = 1.0


# file_path = 'con_vel_basic_log.csv'
# file_path = 'con_vel_beacon_freq_log.csv'
# file_path = 'con_vel_beacon_flag_AND_freq_log.csv'

# file_path = 'dyn_acc_basic_log.csv'
# file_path = 'dyn_acc_beacon_freq_log.csv'
file_path = 'dyn_acc_beacon_flag_AND_freq_log.csv'

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
UKF mit Richtungsvektoren.
"""

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


def h_x(state):

    x, y, vx, vy, _, _ = state

    def normalize_vector(v):
        """
        Wichtig: Nulldivision-Behandlung mit implementiert! Ansonsten meckert die Vorhersage.
        Normalisiert einen Vektor. Gibt Standardvektor zurück, wenn der Eingabevektor ein Nullvektor ist.
        """
        x, y = v
        mag = np.sqrt(x ** 2 + y ** 2)
        if mag < 1e-6:  # Vermeidung Division durch Null
            return np.array([0, 1])  # Oder einen anderen Standardwert
        return np.array([x / mag, y / mag])

    def times(mat, vec):
        return np.array([
            vec[0] * mat[0] + vec[1] * mat[1],
            vec[0] * mat[2] + vec[1] * mat[3]
        ])

    # assign beacon posis
    beacon0_pos = BEACON_POSITIONS[0]
    beacon1_pos = BEACON_POSITIONS[1]
    beacon2_pos = BEACON_POSITIONS[2]

    # calculate vectors in worldspace
    hdg = normalize_vector([vx, vy])
    b0 = normalize_vector([beacon0_pos[0] - x, beacon0_pos[1] - y])
    b1 = normalize_vector([beacon1_pos[0] - x, beacon1_pos[1] - y])
    b2 = normalize_vector([beacon2_pos[0] - x, beacon2_pos[1] - y])

    # ships rotation-mat and its inverser
    ship_mat_ws = np.array([
        hdg[1], +hdg[0],
        -hdg[0], hdg[1]
    ])

    ship_mat_ws_inv = np.array([
        hdg[1], -hdg[0],
        +hdg[0], hdg[1]
    ])

    # convert b0, b1 and b2 from worldspace to ships modelspace
    b0 = times(ship_mat_ws_inv, b0)
    b1 = times(ship_mat_ws_inv, b1)
    b2 = times(ship_mat_ws_inv, b2)

    # hdg[0], hdg[1],
    return np.array([b0[0], b0[1], b1[0], b1[1], b2[0], b2[1]])


z0 = []
z1 = []
z2 = []

for index, row in df.iterrows():
    z0.append([row['Richtung_X_B0'], row['Richtung_Y_B0']])
    z1.append([row['Richtung_X_B1'], row['Richtung_Y_B1']])
    z2.append([row['Richtung_X_B2'], row['Richtung_Y_B2']])

t = [(i * dT) for i in range(len(df))]

z0x = [h[0] for h in z0]
z0y = [h[1] for h in z0]

z1x = [h[0] for h in z1]
z1y = [h[1] for h in z1]

z2x = [h[0] for h in z2]
z2y = [h[1] for h in z2]


# UKF


def get_RMSE_from_Q(Q_variance):

    Q_VARIANCE_VALUE = Q_variance

    sigmas = MerweScaledSigmaPoints(
        n=6, alpha=SIGMA_ALPHA_VALUE, beta=SIGMA_BETA_VALUE, kappa=SIGMA_KAPPA_VALUE)

    ukf = UKF(dim_x=6, dim_z=6, fx=f_x, hx=h_x, dt=dT, points=sigmas)
    # ukf.x = states[0].copy() # ggf. kontrollieren

    # Initialzustand aus dem ersten Logeintrag holen
    init_state = np.array([
        df['X_GT'].values[0], df['Y_GT'].values[0],  # x, y
        0, 0,  # vx, vy
        0, 0   # ax, ay
    ])

    ukf.x = init_state

    ukf.Q = Q_discrete_white_noise(
        dim=2, dt=dT, var=Q_VARIANCE_VALUE, block_size=3, order_by_dim=False)

    # Speichere geschätzten Zustände und Kovarianzmatrizen
    uxs = []
    uPs = []

    # Iteriere durch jede Zeile im Df
    for index, row in df.iterrows():

        # Hole die Standardabweichungen für die aktuelle Messung, verkleine diese, um sie von Grad zu norm. Vektoren anzupassen, das ist nur eine math. Näherung!
        std_b0 = row['STD_Richtung_B0'] / 100
        std_b1 = row['STD_Richtung_B1'] / 100
        std_b2 = row['STD_Richtung_B2'] / 100

        # Zeige Std zwecks Korrektheit
        # print(std_b0, std_b1, std_b2)

        # Setze die Rauschmatrix R basierend auf den extrahierten Standardabweichungen immer neu
        ukf.R = np.diag([std_b0 ** 2, std_b0 ** 2, std_b1 ** 2,
                        std_b1 ** 2, std_b2 ** 2, std_b2 ** 2])

        # Prüfe ob Std. in R-Matrix gesetzt werden
        # print(ukf.R)

        # Hole den aktuellen Messvektor z
        z = np.array([
            row['Richtung_X_B0'], row['Richtung_Y_B0'],
            row['Richtung_X_B1'], row['Richtung_Y_B1'],
            row['Richtung_X_B2'], row['Richtung_Y_B2']
        ])

        # Check ob Daten korrekt eingelesen sind
        # print(z)

        # Vorhersage und Update mit dem aktuellen Messvektor
        ukf.predict()
        ukf.update(z)

        # Speichere die geschätzten Zustände und Kovarianzmatrizen
        uxs.append(ukf.x.copy())
        uPs.append(ukf.P.copy())

    uxs = np.array(uxs)
    uPs = np.array(uPs)

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
