# NifrekaNetTraffic
Version 1.3\
Build 45\
2022-03-02\
[ChangeLog](https://github.com/Col-V/NifrekaNetTraffic/blob/main/ChangeLog.md)

[Download Programm](https://github.com/Col-V/NifrekaNetTraffic/releases)

---
**Systemvoraussetzung:**\
Windows 10 oder Windows 11\
NET 6.0 Runtime
\
\
**Entwicklungsumgebung:**\
NifrekaNetTraffic wird mit Visual Studio 2022 und NET 6.0/C#/WPF entwickelt.

---

| Readme_en | Readme_de|
| --- | --- |
|  [![uk-flag-80x44](https://user-images.githubusercontent.com/32561354/156458311-1e0ad6b0-903f-4b26-90c6-b61a81c84454.png)](https://github.com/Col-V/NifrekaNetTraffic/blob/main/README.md) |  [![de-flag-80x44](https://user-images.githubusercontent.com/32561354/156453083-e689c968-4eab-4d73-8912-f0bd9333c0fd.png)](https://github.com/Col-V/NifrekaNetTraffic/blob/main/README_de.md)  |

---

NifrekaNetTraffic ist ein einfaches WPF-Desktop Programm für Windows 10 und Windows 11 und zeigt Informationen über den Datenverkehr der aktuellen Netzwerkschnittstelle in kompakter, grafischer und tabellarischer Form an.

NifrekaNetTraffic benötigt nur sehr geringe CPU-Ressourcen und kann so auch auf weniger leistungsfähigen Systemen verwendet werden.

Zur Darstellung der Netzwerk-Performance zeichnet NifrekaNetTraffic die Daten etwa alle 1 Sekunde auf, derzeit limitiert auf 86400 Intervalle (= 1 Tag).
\
\
NifrekaNetTraffic beim Beobachten eines Livestreams\
![02](https://user-images.githubusercontent.com/32561354/156446470-0113260b-6930-4d6c-9ebd-c55416013df4.png)
\
\
NifrekaNetTraffic hat zwei Fenster:

- Hauptfenster (mit 3 verschiedenen Ansichten)
    - Kompakt
    - Grafik
    - Grafik + Kompakt
- Fenster Tabelle

\
**Hauptfenster : Kompakt**\
Die kompakte Darstellung hat keine Titelleiste und beansprucht nur wenig Platz auf dem Bildschirm.\
![03-1](https://user-images.githubusercontent.com/32561354/156446529-c23854b2-898d-47b4-8b27-0c14352babe5.png)
\
\
**Hauptfenster : Grafik**\
Darstellung der Logdaten in grafischer Form.\
![03-2](https://user-images.githubusercontent.com/32561354/156446571-528494f6-f8c4-4a11-838a-42b943dc0b08.png)


Mit dem unteren Regler oder den Tasten Pfeil-Links/Rechts kann man den Darstellungsbereich einstellen.

Die Grafik wird automatisch an den maximalen Wert im Darstellungsbereich angepasst. Mit den Reglern an der rechte Seite lässt sich die Darstellung zusätzlich manuell skalieren.

Button "Bit" bzw. "B" schaltet die Darstellung auf Bits oder Bytes um.
\
\
**Hauptfenster : Grafik + Kompakt**\
Kombinierte Darstellung\
![03-3](https://user-images.githubusercontent.com/32561354/156446615-cda51bd2-eff7-4ba5-a3d1-5f521b73f745.png)
\
\
**Fenster : Tabelle**\
Ein optionales Fenster zur Darstellung der Logdaten in einer Tabelle.\
![05](https://user-images.githubusercontent.com/32561354/156446719-736f5d35-9160-48b4-ae88-6f1402e1d27a.png)


Tipp: Die Option "Automatisch rollen" ausschalten ,dann mit SHIFT-Klick mehrere Zeilen gleichzeitig auswählen und die Summe der Selektion im Kopfbereich der Tabelle anzeigen.
\
\
**Als Text exportieren:**\
Speichert die Logdaten im TAB-Format als Textdatei.

Format:
- DataTime_Str <\t>
- BytesReceived <\t>
- BytesSent <\t>
- BytesReceivedInterval <\t>
- BytesSentInterval <\r\n>

\
**Log Ordner:**\
Öffnet den Ordner
Benutzerordner -> Dokumente -> NifrekaNetTraffic -> NifrekaNetTraffic_Data
\
\
**Auf Update prüfen:**\
![06a](https://user-images.githubusercontent.com/32561354/156446769-d636bd3b-f805-4909-bf5f-45527b2e9dca.png)
\
Ist diese Option markiert, wird beim Programmstart automatisch geprüft und angezeigt, ob auf dem NifrekaNetTraffic Server eine neuere Version verfügbar ist.
\
\
**Logs synchronisieren:**\
Mit dieser Option kann man die Anzeige der beiden Log-Fenster synchronisieren.\
![06b](https://user-images.githubusercontent.com/32561354/156446878-e8ff46f3-79e1-4104-81c0-ff5a215adb45.png)
\
\
Ist diese Option markiert, wird die Anzeige des jeweils anderen Log-Fensters auf den gleichen Darstellungsbereich eingestellt.

Tipp: Im Fenster Log-Graph die linke Maustaste gedrückt halten und die Maus bewegen, um den gleichen Darstellungsbereich im Fenster Log-Table kontinuierlich einzustellen.
\
\
**Fensterposition:**\
Mit dem Kontext-Menü (rechte Maustaste) kann man beide NifrekaNetTraffic Fenster auf einmal in einer beliebigen Bildschirmecke anordnen.\
![07](https://user-images.githubusercontent.com/32561354/156446928-6adecfa6-b22f-417f-bfa4-edef15b6566b.png)
\
Bei mehreren Monitoren werden die Fenster dabei in die ausgewählte Bildschirmecke des Monitors bewegt, auf dem sich der Mauszeiger gerade befindet.

Beim Beenden merkt sich NifrekaNetTraffic die letzte Position/den Status der Fenster, so dass NifrekaNetTraffic beim nächsten Start wieder so erscheint, wie man es beendet hat. Es speichert diese Information in der Datei:

Benutzerordner -> Dokumente -> NifrekaNetTraffic -> NifrekaNetTraffic_Data -> NifrekaNetTraffic_Settings.nntDATA

Löscht man diese Datei, erscheint NifrekaNetTraffic beim nächsten Start in der rechten unteren Bildschirmecke.
\
\
**Immer im Vordergrund:**\
Ist diese Option markiert wird das NifrekaNetTraffic Hauptfenster immer über anderen Fenstern angezeigt.
\
\
**Netzwerkschnittstelle:**\
Beim Umschalten der Netzwerkverbindung z.B. Ethernet -> WLAN oder zurück wird dieser Wechsel von NifrekaNetTraffic automatisch erkannt und angezeigt.
\
\
**Digitale Signatur:**\
NifrekaNetTraffic ist nicht digital signiert.

Der Grund ist ganz einfach. Code-Signing kostet leider Geld und außerdem zusätzlichen Zeit-/Wartungsaufwand, was für so ein einfaches Programm wie "NifrekaNetTraffic" unverhältnismäßig ist.

Dies hat zwei für manche Anwender irritierende Effekte:

1. Manche Virus-Scanner betrachten unsignierten Programmcode als gefährlich, da ja dann der Hersteller der Software unbekannt ist und nicht verifiziert werden kann.

2. Auch Windows bringt eine auffällige Warnung, wenn man unsignierte Programme ausführen möchte, die man zuvor aus dem Internet heruntergeladen hat.

Selbstverständlich ist die Verwendung von Programmen aus unbekannter Quelle ein gewisses Risiko, allerdings tauchen auch immer wieder Schadprogramme auf, die eine digitale Signatur haben.

Eine Digitale Signatur ist also keine Garantie für Unbedenklichkeit!
\
\
**Download/Installation/Deinstallation:**\
NifrekaNetTraffic can be downloaded as a zip archive "NifrekaNetTraffic.zip" here:
https://github.com/Col-V/NifrekaNetTraffic/releases

Alle Programmbestandteile von NifrekaNetTraffic sind nach bestem Wissen und Gewissen virenfrei.

Wer möchte kann hier den Source Code von NifrekaNetTraffic herunterladen und das Programm selber compilieren.
https://github.com/Col-V/NifrekaNetTraffic

Bevor NifrekaNetTraffic benutzt werden kann, muss der Inhalt des Zip-Archivs "NifrekaNetTraffic.zip" entpackt werden. Nach dem Entpacken kann das Zip-Archivs "NifrekaNetTraffic.zip" gelöscht werden.

Das Programm "NifrekaNetTraffic.exe" und alle weiteren zugehörigen Programmbestandteile befinden sich innerhalb des Zip-Archivs im Ordner "NifrekaNetTraffic".

Dieser Ordner kann nach dem Entpacken des Archivs an einen beliebigen Speicherort bewegt werden, denn "NifrekaNetTraffic.exe" kann prinzipiell an jedem beliebigen Speicherort ausgeführt werden.

Zur Deinstallation braucht man nur den Ordner "NifrekaNetTraffic" löschen und den gleichnamigen Ordner im Benutzerordner:
"Benutzerordner -> Dokumente -> NifrekaNetTraffic"

---
**Wer unsicher ist, wie man nach dem Herunterladen eines ZIP-Archivs vorgeht, hier einige Hinweise.**
\
\
**Schritt 1.**\
Nach Beenden des Downloads den Download-Ordner öffnen.\
![08](https://user-images.githubusercontent.com/32561354/156447095-3d2eea8b-fba4-4baf-9ae0-48609ab3d2f5.png)
\
\
**Schritt 2.**\
ZIP-Archiv "NifrekaNetTraffic.zip" öffnen.\
![09](https://user-images.githubusercontent.com/32561354/156447143-4273fbec-0083-4734-adb9-7181a12b2ae4.png)
\
\
**Schritt 3.**\
Den Ordner "NifrekaNetTraffic" an einen beliebigen Speicherort (z.B. Dokumente) kopieren.\
![10](https://user-images.githubusercontent.com/32561354/156447177-3a1eeebd-4044-42f7-b624-5a21e88f0d0e.png)
\
\
**Schritt 4.**\
Im Ordner "Dokumente" den kopierten Ordner "NifrekaNetTraffic" öffnen und das Programm "NifrekaNetTraffic.exe" ausführen.\
![11](https://user-images.githubusercontent.com/32561354/156447218-1fbfdebd-297b-4c98-95a9-568707a12e61.png)
\
\
**Schritt 5.**\
Warnung (wegen nicht vorhandener digitaler Signatur) behandeln: Auf "Weitere Informationen" klicken.\
![12](https://user-images.githubusercontent.com/32561354/156447249-8e4ffbba-2505-4add-8d54-41bae1d8a4e1.png)
\
\
**Schritt 6.**\
Auf "Trotzdem ausführen" klicken und "NifrekaNetTraffic.exe" wird gestartet.\
![13](https://user-images.githubusercontent.com/32561354/156447282-ad6581c6-d3ac-4d07-a8de-80072746781f.png)
\
\
**Schritt 7.**\
Damit man "NifrekaNetTraffic.exe" jederzeit schnell ausführen kann, am besten die üblichen Windows-Techniken verwenden wie z.B.

- "NifrekaNetTraffic.exe - Verknüpfung" auf dem Desktop anlegen
- An Taskleiste anheften
- An "Start" anheften

**Schritt 8.**\
Das Zip-Archiv "NifrekaNetTraffic.zip" wird nach dem Entpacken nicht mehr benötigt und kann gelöscht werden.


---
**Autostart:**\
Um NifrekaNetTraffic.exe beim Starten von Windows automatisch auszuführen, kopiert man eine Verknüpfung von NifrekaNetTraffic.exe in den Ordner "Startup".

"C:\Users\User11\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\"
(User11 = Benutzername)
Benutzerordner > AppData > Roaming > Microsoft > Windows > Startmenü > Programme > Autostart

NifrekaNetTraffic.exe - Verknüpfung im Ordner Startup (Autostart)\
![14](https://user-images.githubusercontent.com/32561354/156447329-3a5fea04-4523-43fd-ad6a-2ebd9306cb7a.png)


Der Ordner "AppData" wird im Benutzerordner nur angezeigt, wenn in
- Systemsteuerung -> Explorer-Optionen -> Ansicht

die Option
- "Ausgeblendete Dateien, Ordner und Laufwerke anzeigen"

markiert ist.

![15](https://user-images.githubusercontent.com/32561354/156447375-88e7136c-a141-41f5-b67f-9b365543c2bd.png)
---
# NET 6.0 Runtime Installation
NifrekaNetTraffic benötigt NET 6.0 Runtime.

Sollte NET 6.0 Runtime auf dem Computer noch nicht installiert sein, wird Windows darauf hinweisen und den Download von microsoft.com anbieten.

![16](https://user-images.githubusercontent.com/32561354/156447403-f76267c4-620f-4d73-bfae-b072a541f824.png)

![17](https://user-images.githubusercontent.com/32561354/156447423-1a6d510b-65e3-49af-ae04-5f498b28aa14.png)

Die heruntergeladene Datei öffnen und die Installation durchführen.

---
nifreka.nl



