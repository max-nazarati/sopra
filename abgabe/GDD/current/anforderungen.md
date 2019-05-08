## Anforderungen
> „Anforderungen legen die qualitativen und
quantitativen Eigenschaften eines Produkts aus der
Sicht des Auftraggebers fest.“

###3 Arten von Anforderungen

* **Funktionale Anforderungen** definieren die funktionalen Effekte, die eine Software auf ihre Umgebung ausüben soll.
* **Qualitätsanforderungen** beschreiben zusätzliche Eigenschaften, die diese funktionalen Effekte haben sollen.
* **Randbedingungen** beschränken die Art auf die die SW
funktionale Anforderungen erfüllt oder auf die die SW
entwickelt wird.

#### Funktionale Anforderungen
- [ ] 2D oder 3D Grafik (kein ASCII).
- [ ] Min. 2 Spieler, min. einer davon „menschlich“.
- [ ] [Indirekte Steuerung][Steuerung] (Point & Click).
- [ ] Potenziell zu jedem Zeitpunkt speichern/laden, muss aber nicht zwangsläufig vom Spieler gesteuert sein.
- [ ] Pausefunktion.
- [ ] Eigenes [Menü][Menü] (komplett mit der Maus steuerbar,
außer Texteingaben).
- Arten von [Spielobjekten][Spielobjekte]:
 - [ ] a) Min. 5 Kontrollierbare.
 - [ ] b) Min. 5 Auswählbare.
 - [ ] c) Min. 5 Nicht-Kontrollierbare, davon min. 3 Kollidierende.
 - [ ] d) Min. 3 Kontrollierbare, Kollidierende und Bewegliche.
- [ ] Min. 1000 gleichzeitig [aktive Spielobjekte][aktive Spielobjekte] der Art (d) möglich (Tech-Demo).
- Arten von [Aktionen][Aktionen]:
 - [ ] Min. 10 verschiedene Aktionen (inkl. Laufen, Fähigkeiten, usw.).
 - [ ] Allen Spielobjekten der Art [(d)][aktive Spielobjekte] muss es möglich sein, von jedem beliebigen Punkt in der Welt zu jedem anderen begehbaren Punkt zu gelangen, ohne sich gegenseitig übermäßig zu behindern, festzustecken, usw. („Pathfinding“).
- [ ] Soundeffekte und Musik.
- [ ] Min. 5 verschiedene [Statistiken][Statistiken].
- [ ] and finally [Achievements][Achievements].

Sind die Softwarequalitätsanforderungen erfüllt (d.h. gibt es
keine Compiler/ReSharper Warnungen oder Fehler)?

#### Qualitätsanforderungen
* Entwickeln Sie ein _gutes_ Produkt.
* Qualität der Grafik ist nicht relevant.
* Grafiken sollen in sich stimmig sein.
* Akustische Effekte sollen in sich stimmig sein.
* Die im Spiel enthaltenen Texte müssen frei von Rechtschreib-, Grammatik- und Umlautfehlern sein.
* Richtlinen zur Bedienbarkeit von Computerspielen beachten [Usability beim Spieldesign][Usability]).

#### Randbedingungen
* Programmiersprache C# und/oder F# mit .NET.
* MonoGame 3.7.
* Auf Windows 7 x86/x64 lauffähig.
* Visual Studio Community 2019.
* Keine Warnings oder Errors vom Compiler oder ReSharper (wöchentlich), keine Buildfehler.


[Steuerung]:./benutzeroberflaeche/menu.tex
[Menü]: ./benutzeroberflaeche/menu.tex
[Spielobjekte]: ./spiellogik/objekte.tex
[Aktionen]: ./spiellogik/optionen-aktionen.tex
[aktive Spielobjekte]: ./benutzeroberflaeche/menu.tex
[Statistiken]: ./spiellogik/statistiken.tex
[Achievements]: ./spiellogik/achievements.tex
[Usability]: https://sopra.informatik.uni-freiburg.de/soprawiki/UsabilityForGames
