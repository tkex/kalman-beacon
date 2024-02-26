# kalman-beacon


## Struktur

- **Root**: Unity 2D Boot Simulations mit Sensor-Log-Erzeugung
- **LaTeX**: Backup unserer LaTeX-Dokumente
- **__PythonPrototyping__/UKF**: Unsere UKF-Implementierungen, Visualisierungen sowie Log-Daten
- **kalman-beacon-hdrp**: 3D Implementierung in Unity

## Liste der derzeit implementierten (und geplanten) Funktionen

Hier sind einige Hinweise zu den Funktionen, die (noch) nicht implementiert sind.

Die folgende Liste der Funktionen soll in Zukunft im Kalman-Beacon-Projekt implementiert werden:

### Unscented Kalman-Filter:

| Name | Beschreibung | Status / Anmerkungen |
| --- | --- | --- |
| Import von Bibliotheken | Integration der (notwendigen) Bibliotheken wie NumPy, Matplotlib und FilterPy. | ✅ DONE |
| Konstantendefinition | Definition der Konstanten für Beacon-Positionen und Zeitintervall. | ✅ DONE |
| Zustandsübergangsfunktion | Implementierung der Funktion zur Berechnung des neuen Zustands. | ✅ DONE |
| Messfunktion | Entwicklung der Funktion zur Umwandlung des Zustandsraums in Messwerte. | ✅ DONE |
| Schiffsbewegungssimulation | Simulation der Schiffsbewegung über einen definierten Zeitraum. | ✅ DONE |
| Erzeugung verrauschter Messdaten | Funktion zur Erstellung von verrauschten Messdaten aus den wahren Zuständen. | ✅ DONE |
| UKF-Initialisierung | Aufbau und Konfiguration des Unscented Kalman Filters. | ✅ DONE |
| Geschwindigkeitsaktualisierung | Implementierung für die Aktualisierung der Geschwindigkeit (im UKF) | 🚧️ ***Under construction***  (Refaktorisierung in Erwägung) |
| Hauptfunktion | Hauptfunktion mit Initialisierung, Vorhersage und Update-Zyklen. | ✅ DONE |
| Visualisierung | Entwicklung einer Methode zur Visualisierung der Ergebnisse des Filters. | 🚧️ ***Under construction*** |
| Leistungsanalyse | Implementierung von Tests zur Bewertung der Leistung und Genauigkeit des Filters. | ❌ **TODO** |
| Parametrisierung / Tuning | Anpassung der Filterparameter für optimalere Leistung. | ✅ DONE |
| Fehlerbehandlung | Erweiterung des Codes um robuste Fehlerbehandlung und Ausnahmeprüfungen. | ❌ **TODO** |
| Echtzeit | Erweiterung/Anpassung des Codes zwecks Echtzeit-Datenverarbeitung. | 🚧️ ***Under construction*** |
| Dokumentation | Erstellung eines Papers zum Projekt | ✅ DONE |

### Unity:

| Name | Beschreibung | Status / Anmerkungen |
| --- | --- | --- |
| Unity-Projektsetup | Einrichtung des Unity-Projekts. | ✅ DONE |
| Sensordatenintegration | Integration und Verarbeitung von Sensordaten. | ✅ DONE |
| Bootssimulation | Entwicklung einer realistischen Bootssteuerungs- und Bewegungssimulation (Physik). | 🚧️ ***Under construction*** |
| Physik-Engine Anpassung | Verwendung der Unity-Physik-Engine für realistische Bootsbewegungen. | 🚧️ ***Under construction*** |
| 3D-Modellierung | Erstellung oder Integration von 3D-Modellen für das Boot und die Umgebung. | ❌ **TODO** |
| Sensordatenvisualisierung | Entwicklung von Methoden zur Visualisierung der Sensordaten in Echtzeit. | 🚧️ ***Under construction*** |
| Logdateierstellung | Implementierung eines Systems zur Erstellung von Logdateien für Sensordaten | ✅ DONE |
| WebSocket-Implementierung | Aufbau einer WebSocket-Verbindung für Echtzeit-Datenübertragung. | ✅ DONE |
| Benutzeroberfläche | Entwicklung einer Benutzeroberfläche zur Anzeige von Sensordatenn. | ✅ DONE |
| Echtzeit-Interaktion | Entwicklung von Echtzeit-Interaktion für das Boot. | 🚧️ ***Under construction*** |
| Fehlerbehandlung und Optimierung | Implementierung robuster Fehlerbehandlung und Leistungsoptimierung. | ❌ **TODO** |

<sub> **NOTE:** Last update on 31-01-2024 (list will be updated). </sub>

### Status-Legende:
 
 - ❌️ **TODO** - diese Funktion **kann** oder **kann nicht** in Zukunft implementiert werden
   Zukunft. 

- ⚠️ **Will do** - diese Funktion ist noch nicht im Code implementiert, aber
   es ist als nächstes Feature geplant, an dem gearbeitet wird.

 - ✅️ **DONE** - diese Funktion wurde vollständig implementiert und es besteht keine Notwendigkeit
   aktiv daran zu arbeiten.
 
 - ⛔️ **Broken** - an dieser Funktion wurde gearbeitet, aber sie funktioniert seit kurzem nicht mehr
   aufgrund von unbekannten Problemen.

 - 🚧️ ***Under construction*** - Eine (größere) Überarbeitung dieser Funktion ist in
   in Arbeit, um sie zu verbessern.
