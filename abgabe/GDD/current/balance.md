## <center> Werte und Zahlen die für ein balanciertes Spielerlebnis relevant sind </center>

Abkürzung  |  Attribut                     | Angriff / Verteidigung
-----------|-------------------------------|:----------------------:
K          | Kosten                        | A und V
VS         | Verteidigungsstärke           | V
AI         | Angriffsintervall             | V
RW         | Reichweite                    | V
LP         | Lebenspunkte                  | A
AS         | Angriffsstärke                | A
GS         | Geschwindigkeit               | A
F          | Fähigkeit                     | A (nur Helden)

### Bitcoins

Einnahmequelle   |  Wert
-----------------|--------
Startkapital     |  100
Gold pro Sekunde |  (int) vergangene Minuten


### Verteidigungsgebäude

Gebäude                  |   K   |  VS  |  AI |  RW |
-------------------------|-------|------|-----|-----|
Kabel                    |   20  |   -  |  -  |  -  |
Mauszeigerschütze        |   40  |   2  |  1  |  6  |
CD-Werfer                |   50  |   5  |  2  |  4  |
Antivirusprogramm        |   60  |   8  |  3  | 10  |
Lüftung                  |   50  |   0  |  -  |  3  |
WiFi-Router              |   80  |   1  |  1  |  2  |
Schockfeld               |  100  |   2  |  3  |  -  |

#### Upgrades für Verteidigungsgebäude:

Mein Gedanke ist:
Jedes Gebäude bekommt ein Upgrade mit Kosten 5, dass den Turm ungefähr doppelt so gut macht.

Gebäude            | Preis | Name          | Effekt                                        |
-------------------|-------|---------------|-----------------------------------------------|
Kabel              |   5   | Kabelsalat    | Kabel kosten die Hälfte                       |
Mauszeigerschütze  |   5   | Doppelklick   | Es werden zwei-Projektil-Salven geschossen    |
CD-Werfer          |   5   | Boomerang     | Die CD fliegt nach erreichen der Range zurück |
Antivirusprogramm  |   5   | Früherkennung | Erhöht die Reichweite (Radius) um 50%         |
Lüftung            |   5   | Schneesturm   | Lüftung verursacht Schaden im Effektbereich   |
WiFi-Router        |   5   | 5 GHz         | Halbiert die Dauer zwischen zwei Angriffen    |
Schockfeld         |   5   | Schnelllader  | Halbiert die Dauer zwischen zwei Angriffen    |

#### Verbesserungen für Verteidigungsgebäude:

Mein Gedanke ist:  
Um es möglichst unkompliziert zu belassen, werden nur die Schadenswerte angepasst.  
Die Kosten je Upgrade sollen 100% der Turmkosten entsprechen.  
Die Preis-Leistung soll ungefähr auf 75% (stufe2) bzw 50% (stufe3) sinken.  
Beispielszahlen:  
Ein Turm kosten 10, macht 10 Schaden.  
Upgrade auf Stufe 2 kostet 10 erhöht schaden um 7.5  
Upgrade auf Stufe 3 kostet 10 erhöht schaden um 5

Gebäude            | Effekt Stufe 2         | Effekt Stufe 3                   |
-------------------|------------------------|----------------------------------|
Kabel              | -                      | -                                |
Lüftung            | ?                      | ?                                |
Mauszeigerschütze  | + 75% Basis-Schaden    | + 50% Basis-Schaden              |
CD-Werfer          | + 75% Basis-Schaden    | + 50% Basis-Schaden              |
Antivirusprogramm  | + 75% Basis-Schaden    | + 50% Basis-Schaden              |
WiFi-Router        | + 75% Basis-Schaden    | + 50% Basis-Schaden              |
Schockfeld         | + 75% Basis-Schaden    | + 50% Basis-Schaden              |

### Angriffseinheiten
#### Truppen

Truppe        |  K  |  LP  |  AS |  GS |  F
--------------|-----|------|-----|-----|------------------------
Bug           |  2  |   4  |  1  |  4  |   -
Virus         |  3  |  10  |  2  |  3  |   -
Trojaner      | 30  |  30  |  6  |  2  | spawnt beim Tod 5 Bugs
Nokia         | 50  | 100  | 15  |  1  |   -
Thunderbird   | 15  |  15  |  3  |  2  | fliegt


Helden      |  K  |  LP  |  AS |  GS |  F      |
------------|-----|------|-----|-----|---------|
Settings    | 50  |  25  |  0  |  3  |  heal   |
Firefox     | 50  |  30  | 10  |  5  |  jump   |
Bluescreen  | 50  |  15  |  0  |  6  |  emp    |

### Upgrades
(NOT UP TO DATE)
Diese kosten Erfahrungspunkte anstatt Bitcoins (Belohnung für Bestehen einer Angriffswelle)

K   | Effekt
----|-------------
1   | LP aller Einheiten um 5 % verbessern.
1   | GS aller Einheiten um 5 % verbessern.
1   | VS aller Gebäude um 5 % verbessern.
1   | AI aller Gebäude um 5 % verbessern.
    |
2   | LP aller Einheiten 10 % verbessern.
2   | VS aller Gebäude 10 % verbessern.
2   | GS aller Einheiten um 10 % erhöhen.
2   | 10 % mehr Bitcoin pro Sekunde.
    |
3   | _CD-Werfer_ schießt CD’s als Boomerang.
3   | GS von _Nokia_ um 40 % erhöhen.
3   | GS von _Firefox_ wird um 10 % erhöht.
3   | _Trojaner_ transportieren 5 Einheiten mehr.
    |
4   | Möglichkeit, bis zu zwei Firefox-Einheiten gleichzeitig zu kontrollieren.
4   | EMP-Effekt von Bluescreen dauert 50 % länger.
4   | Trojaner transportieren 10 Einheiten mehr.
4   | Einzugsbereich von Settings um 5 % größer.
4   | Heil-Rate von Settings um 5 % erhöhen.
    |
5   | Bluescreen hat einen zweiten EMP-Angriff, bevor eine Aufladung nötig ist.
5   | Möglichkeit, bis zu zwei Firefox-Einheiten gleichzeitig zu kontrollieren.
5   | Einzugsbereich von Settings um 10 % größer.
5   | Heil-Rate von Settings um 10 % erhöhen.
