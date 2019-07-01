## Prototype WPF PETSCII Textbox Control
Das Projekt verwendet die Schriftart "C64 Pro Mono" ( https://style64.org/c64-truetype ), welche installiert sein muss.
![](/images/Prototype_WPF_PETSCII_Textbox_Control_001.png)

## Was ist das Ziel dieses Projektes?
Dieses Projekt zeigt, wie eine Eingabe von PETCII in einem Disk Image Tool aussehen könnte.
Diese Eingabe könnte zum Ändern von Dateinamen oder Diskettennamen oder Verzeichnisnamen verwendet werden.
Die Darstellung des PETCIIs erfolgt über die True Type Schriftart "C64 Pro Mono" ( https://style64.org/c64-truetype ).

## Eigenschaften
- Ausgabe eines Zeichen für alle 256 Codes des PETSCIIs (die nicht darstellbaren Steuerzeichen werden als "Rechteck" dargestellt)
- die Ausgabe kann mit Zeichensatz 1 oder in Zeichensatz 2 erfolgen
- es kann zwischen Zeichensatz 1 oder in Zeichensatz 2 umgeschaltet werden
- es können 87 Zeichen mit einer Tastatur eingeben werden
- die Eingabe von anderen Zeichen wird unterbunden
- die Eingabe von Emojis wird unterbunden
- die Eingabemethode über Alt-Taste und Ziffern am Ziffernblock wurde deaktiviert
- die Eingabe über speziellen Eingabeprogramm (Input Method Editor, IME) wurde deaktiviert
- kopieren und einfügen bzw. ziehen und ablegen aller 256 Codes des PETSCIIs innerhalb der der Anwendung, über einen eigenen Datenformatnamen "PETSCII"
- kopieren bzw. ziehen aller 256 Codes des PETSCIIs als Unicode (E0XX bzw. E1XX) zu einem unbekannten Ziel möglich
- kopieren bzw. ziehen von 87 Codes des PETSCIIs als Unicode (00XX) bzw. ANSI (XX) zu einem unbekannten Ziel möglich
- einfügen bzw. ablegen aller 256 Codes des PETSCIIs als Unicode (E0XX bzw. E1XX) aus einer unbekannten Quelle möglich
- einfügen bzw. ablegen von 87 Codes des PETSCIIs als ANSI (XX) aus einer unbekannten Quelle möglich
- einfügen bzw. ablegen aller 256 Codes des PETSCIIs als Unicode (E0XX bzw. E1XX) oder 87 Codes des PETSCIIs als ANSI (XX) aus einer unbekannten Quelle möglich
- ein PopUp erscheint, wenn nicht alle Zeichen beim Einfügen bzw. beim Ablegen verwendet werden konnten
- das WPF TextBox Steuerelemente besitzt einen Tooltip, welcher die Unicode- (E0XX bzw. E1XX) und die PETSCII-Werte des eingegebenen Textes hexadezimal anzeigt

## Was waren die Probleme bei der Entwicklung des Projektes?
Das WPF TextBox Steuerelemente verfügt über eine Vielzahl von integrierten Funktionen, welche alle betrachtet werden müssen.

Es war auch schwierig den geeigneten Weg zu finden, wie die Zuordnung einer Taste (bzw. dessen Zeichen) zu einem PETSCII Code erfolgen soll.
Die Position einer Taste auf einer Tastatur kann nur schwierig ermittelt werden.
Dazu müsste das Programm das Layout der benutzten Tastatur genau kennen.
Diese Layouts müsste für die gängigsten Tastaturen bereitgestellt werden, so dass diese dann vom Benutzer ausgewählt werden können.
Beim VICE Emulator gibt es dafür spezielle Tastatur Zuordnungs-Dateien (keyboard mapping file z.B. **win_sym_de.vkm** **win_pos_de.vkm**).
Deshalb wurde in diesem Projekt darauf verzichtet, über die Position von Tasten eine Ermittlung des entsprechenden PETCII Codes durchzuführen.
Dieses Projekt benutzt nur das Zeichen, welches eine Taste liefert für die Ermittlung des entsprechenden PETCII Codes.
Dabei ist die Position der Taste nicht von Bedeutung.

Die Ermittlung von reinen Tastatur Scan Codes ist mit C# schwierig.
Die reinen Tastatur Scan Codes werden wohl schon vom Tastatur Treiber (Keyboard.dll) in virtuelle Tastatur Codes umgewandelt.
Diese virtuelle Tastatur Codes werden dann (wohl durch die TranslateMessage-Funktion) in Zeichen Codes umgewandelt.
In C# sind die Ereignisse **PreviewTextInput** und **PreviewKeyDown** des WPF TextBox Steuerelementes interessant.
Im **PreviewTextInput** Ereignis steht das eingegebene Zeichen (bzw. die eingeben Zeichen) in den **TextCompositionEventArgs** zur Verfügung.
Auf den virtuellen Tastatur Code hat man wohl im **PreviewTextInput** Ereignis keinen Zugriff.
Leider wird das **PreviewTextInput** Ereignis nicht bei der einer Betätigung der **Leertaste** ausgelöst.
Aus diesem Grund muss noch das **PreviewKeyDown** Ereignis für die Behandlung der **Leertaste** benutzt werden.
Im **PreviewKeyDown** Ereignis kann auf den virtuelle Tastatur Code zugegriffen werden.
In den KeyEventArgs entspricht der Wert **Key** dem virtuellen Tastatur Code.
Leider ist es wohl sehr schwierig das entsprechende Zeichen aus einem virtuellen Tastatur Code zu bestimmen.
Es könnte zwar die Windows Funktion ToUnicodeEx() (über die Runtime InteropServices) verwendet werden, diese bereitet aber auch wieder nur Probleme.
Der Aufruf der Funktion ToUnicodeEx() zerstört die Information der **Tottasten** ( https://de.wikipedia.org/wiki/Tottaste ).

Ja, was sind nun schon wieder **Tottasten**?
Das sind die Tasten, welche nach dem Drücken keinen Buchstabenvorschub erzeugen, sondern eine Art **Wartemodus** aktivieren.
Erst ein weiterer Tastendruck erzeugt dann ein kombiniertes Zeichen (Grundzeichen und Diakritika – wie è, ĉ, ñ, ś oder ů).
Auf normalen deutschen Tastaturen sind das die Zirkumflex-Taste **^**  und die Akzent-Taste **´**.
Wegen den beschrieben Problemen, wurde in diesem Projekt auf die Verwendung der Windows Funktion ToUnicodeEx() verzichtet.
Das bedeutet dann aber, das die eingegebenen Zeichen über das 'PreviewTextInput** Ereignis und die **Leertaste** über das **PreviewKeyDown** Ereignis behandelt werden muss.
Leider müssen so beide Ereignisse benutzt werden, ich hätte gern auf eines der beiden Ereignisse verzichtet.

Ein Test der Anwendung mit der Windows 10 Bildschirmtastatur zeigte, das noch einige wenige Emojis in das WPF TextBox Steuerelement eingeben werden konnte.
So konnte das Rote Herz Emoji ❤ eingeben werden. Zur Anzeige kam wohl das Unicode-Zeichen U+2764 (HEAVY BLACK HEART).
Dabei habe ich festgestellt das es bei Windows 10 auch dein Emoji-Panel (aufrufbar über die Tastenkombination **Windows** + **.**) zur Eingabe von Emojis gibt.
Einige der eingegebenen Emoji werden wohl bei der Eingabe in 16 Bit Unicode-Zeichen umgewandelt.
So wohl auch das Rote Herz Emoji ❤, welche zum Unicode-Zeichen U+2764 umgewandelt wurde.

Andere Emojis werden als 32 Bit Unicode-Zeichen abgelegt (UTF-32).
Ein C# String arbeitet aber nur mit einer 16 Bit Unicode Darstellung (UTF-16).
Unicode-Zeichen außerhalb der Basic multilingual plane (non-BMP U+10000 bis U+10FFFF) können durch zwei 16-Bit-Wörter (engl. code units) dargestellt, werden.
Dazu werden sogenannte **Surrogate Pairs** verwendet ( https://de.wikipedia.org/wiki/UTF-16#Kodierung ).
Um die Eingabe von unerwünschten Emojis zu verhindern wurde die Eigenschaft **IsInputMethodEnabled** des WPF TextBox Steuerelementes auf **false** gesetzt.
Dadurch wird wohl auch die Eingabemethode über Alt-Taste und Ziffern am Ziffernblock deaktiviert ( https://de.wikipedia.org/wiki/Eingabemethode#Tastenkombinationen_bzw._-k%C3%BCrzel ).

### Testeingabe
![](/images/Prototype_WPF_PETSCII_Textbox_Control_002.gif)

### Testeingabe von 87 Zeichen

![](/images/Prototype_WPF_PETSCII_Textbox_Control_003.gif)

| Zeichen               | Zeichenanzahl           |
|:----------------------|:-----------------------:|
| 1234567890!"$%&/()=?  | 20                      |
| qwertzuiop+QWERTZUIOP*| 22                      |
| asdfghjkl#ASDFGHJKL'  | 20                      |
| <yxcvbnm,.->YXCVBNM;: | 21                      |
| @[]                   | 3                       |
| Leertaste             | 1 (fehlt im GIF-Video)  |


### Umschaltung zwischen Zeichensatz 1 und Zeichensatz 2

![](/images/Prototype_WPF_PETSCII_Textbox_Control_004.gif)

### Tooltips

![](/images/Prototype_WPF_PETSCII_Textbox_Control_005.png)
![](/images/Prototype_WPF_PETSCII_Textbox_Control_006.png)