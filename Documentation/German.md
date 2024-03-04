# Dokumentation von SciChat

## Das Chatprogramm
### Nutzung von SciChat als regulärer Nutzer
### Die Dateien
Das Speichern der Daten funktioniert über MySQL. Während der Entwicklung wird aktuell jeweils ein Datenbankserver auf der lokalen Maschine des jeweiligen Entwicklers verwenden.
Im späteren Verlauf soll die Datenbank auf einem Server gehosted werden. 
#### DataBaseHelper.cs
In der Datei [DataBaseHelper.cs](https://github.com/Levithan7/SciChat/blob/main/SciChatProject/DataBaseHelper.cs) befindet sich der Kern der Datenbank Funktionalität. Für unsere Zwecke hat es sich als am sinvollsten herausgestellt eigene Methoden zu implementieren, die die Datenbank
abfragen anstatt für jede Abfrage spezifische SQL Statements zu schreiben. Dabei wurde die Datei in zwei Regionen eingeteilt
##### READER
**GetValuesByQuery(string query)**

Diese Methode liest die Datenbank entsprechend eines bestimmten SELECT queries aus und und gibt die Werte als Liste von Dictionaries zurück. Ein Dictionary ist dabei eine Zeile nach dem Muster <Spaltenname,Wert>.
So gibt die Methode die Datenbankeinträge entsprechend noch als reine Werte zurück.

**GetObjectByValue(Dictionary<string, dynamic> attributePairs, Type type)**

Diese Methode wandelt eine Solche Zeile in ein C# Objekt des Types type um. Dabei wird eine Instanz des Typen type erstellt und dieser Instanz für jedes Attribut, das sie besitzt der entsprechende Wert der Zeile zugewiesen,
wenn dieses Attribut Teil der Spalten ist. In der `Beispiel.cs` des jeweiligen Typen können Attribute dabei mit der dafür von mir erstellten Eigenschaft SQLProperty versehen werden über die es möglich ist dem Namen
des Attributes einen anderen zu geben als in der Datenbank selbst. BEISPIEL DAFÜR EINFÜGEN!!!!

**GetObjectsByQuery<T>(string query)**

Hier wird nun `GetValuesByQuery(string query)` aufgerufen und jeder daraus resultierende Eintrag in `GetObjectByValue(Dictionary<string, dynamic> attributePairs, Type type)` gegeben, sodass die Methode letzendlich anhand eines queries eine Liste von C# Objekten zurückgibt.

##### FILLER
**ExecuteChange<T>(string dataBaseName, List<T> objects, ChangeType changeType = ChangeType.Insert)**
Diese Methode nimmt den Namen der Datenbank an und kann - je nach ChangeType - entweder die Liste der beigefügten Objekte in die entsprechende Datenbank hinzufügen oder die entsprechende Datenbank updaten. Letztere Funktionalität wurde noch nicht implementiert.

**CreateQueryForChange<T>(string dataBaseName, List<T> objects, ChangeType changeType=ChangeType.Insert)**
Diese Methode erstellt anhand der angegebenen Parameter ein Query, welches zum manipulieren von Daten verwendet werden kann. Diese Funktionalität
ist primär dazu vorhanden, um dem Entwickler das Erstellen von Queries abzunehemn.

**List<PropertyInfo> GetListOfChangableProperties(object obj)**
Diese Methode gibt für das Objekt obj alle Properties zurück, die das manuell erstellte Attribut SQLProperty enthalten. In der Definierung eines Models besteht so die Option Properties als SQLProperty zu markieren. Als SQLProperty markierte Properties müssen sich in der Datenbank als Spalte auffinden.

**List<string> GetListOfPropertieNames(object obj)**
Deklariert man das Propertiy eines Model als SQLProperty so besteht dabei die Option den Variablennamen ungleich den Namen der Spalte in der Tabelle zusetzen.
BEISPIEL EINFÜGEN
Diese Methode gibt die Namen dieser Properties zurück.

**List<string> GetListOfPropertieValues(object obj)**
Deklariert man das Propertiy eines Model als SQLProperty, so besteht dabei die Option den Variablennamen ungleich den Namen der Spalte in der Tabelle zusetzen.
BEISPIEL EINFÜGEN
Diese Methode gibt die Werte dieser Properties zurück.

**string? ModifyProperty(dynamic? input)**
Diese Methode wandelt den Wert eines Properties so um, dass dieser in SQL verwendbar ist.
Zahlen bleiben dabei gleich. Um Strings jedoch werden Anführungszeichen gesetzt.

**enum ChangeType**
Dieser enum deklariert alle Änderungstypen. Aktuell sind nur Insert und Update vorhanden wobei nur für Insert eine Funktionalität implementiert wurde.

#### Models.Attributes.cs
Hier wird das C# Attribut SQLProerty definiert. Dessen Verwendungszweck wurde bereits erläutert.

#### Models.SQLClass.cs
In dieser Datei befindet sich eine Vorlagen-Klasse von der Alle Objekte erben müssen, die in die Datenbank eingetragen könenn werden sollen.

#### Weitere Klassen in Models
Objekte dieser Klassen lassen sich in die Datenbank einpflegen. Die meisten der Klassen beinhalten nur sehr einfach gestrickte Methoden deren weitere
Erläuterung nicht notwendig ist. Im allgemeinen geht es oft darum Objekte dieser Klassen anhand der entsprechenden Methodenparamter zu erhalten oder zu modifizieren abhänhig. Eine Ausnahme spielt dabei die Message Klasse.

#### Models.Message.cs
Neben den herkömmlichen Methoden (vgl. Weitere Klassen in Models) bietet Message.cs zusätzlich einige Methoden, die sich auf das Darstellen von Nachrichten im Chat beziehen.
Dabei können sowohl reguläre LaTeX ausdrücke verwendet werden als auch die selbst entwickelte Darstellung von Graphen. Die Verwendung dieser kann in "Nutzung von SciChat durch einen regulären Nutzer" nachgelesen werden.

**string ParseAllCommands(string msg)**
Das ist der Kopf der Funktionalität. Es wurde eine "rohe" Nachricht eingegeben und die entsprechend umgewandelte (ab jetzt: geparsd) Nachrichten werden zurückgegeben.

**void ParseLaTex()**
Wird auf ein Message-Objekt die Methdie ParseLatex() so wird ihr Content ensprechend ParseLaTex(string latex) angepasst.

**string ParselaTeX(string latex)**
Über RegEx (Regular Expressions) wird geprüft, ob ein mögliches LaTeX Element in der Nachricht vorhanden ist. Mögliche LaTeX Elemente beginnen und enden stets mit $$
Jedes dabei gefundene Element wird danach wird über einen eExternen Anbieter (codecogs.com) in ein entsprechendes Bild gewandelt, welches innerhalb der Nachricht dargestellt wird als wäre es
normaler Text-Bestandteil.

**void ParseGraphBuilder()**
Wird auf ein Message-Objekt die Methdie ParseGraphBuilder() so wird ihr Content ensprechend ParseGraphBuilder(string msg) angepasst.

**string ParseGraphBuilder()**
Über RegEx (Regular Expressions) wird geprüft, ob ein mögliches LaTeX Element in der Nachricht vorhanden ist. Mögliche GraphBuilder Elemente beginnen mit `\begingraph` und enden mit `\endgraph`.
Jedes dabei gefundene Element wird danach über die Methode `CreateGraph(string gb)` in ein entsprechendes Bild gewandelt, welches innerhalb der Nachricht dargestellt wird.

**string CreateGraph(string gb)**
Diese Methode ist dafür zuständig aus einem sog. GraphBuilder (kurz: `GB` bzw. `gb`) die nötigen Informationen herauszuziehen und entsprechend einen Graphen zu erstellen.
Dabei gibt es zunächst die Region `graphsettings` in der alle Einstellungen zum Graphen (Typ, Skalierung etc.) eingebracht werden.
Darauf folgt die Region `data` inder die Daten aus der Eingabe geholt werden. Hierbei gibt es verschiedene Optionen Daten anzugeben, die in "Nutzung von SciChat durch einen regulären Nutzer" erläutert werden.
Zum Schluss kommt die Region `plotcreation` in der - je nach Plottype - ein bestimmter Graph erstellt und in das HTML-Image Format umgewandelt wird.

**Func<double, double> StringToLambda(string expression)**
Diese Methode wandelt einen mathematischen der C#-Syntaxt enstprechenden string in eine Func um, die verwendet werden kann, um Graphen per Funktionen `f(x) = ...` zu erstellen.

**string MathToLambda(string math)**
Da Menschen in der Norm ihre Mathematischen Ausdrücke nicht der C#-Syntax entsprechend ausdrücken, müssen einige Ausdrücke umgewandelt werden. Ein Beispiel dafür ist, dass wir `cos(x)` schreiben würden,
C# allerdings `Math.Cos(x)`. Problematisch wurde es vor allem beim kompilieren von Funktionne mit Exponenten, da in C# die schreibweise `Math.Pow(a, b)` statt `a^b` verwendet wird. Darüber ergaben sich dann einige Probleme mit dem Unterscheiden von Exponenten und Basen (v.a. bei Exponenten, die wiederrum Exponenten enthalten etc.), die über die folgenden Methoden abgewickelt werden.

**int FindContraParan(int idx, string text, int direction, bool returnCharacterIfNoParan=false)**
Diese Methode gibt vereinfacht ausgedrückt für eine sich öffnende Klammer an der Stelle `idx` die Stelle zurück in der die Klammer wieder geschlossen wird.

**string ConvertExponentToPow(string paranthase)**
Diese Methode wandelt die Schreibweise von `a^b`in `Math.Pow(a, b)` um.

**string ConvertCurrentLevelParantatheses(string level)**
Diese Methode wandelt das aktuelle Klammern "Level" entsprechend ConvertExponent um. Dabei wird per Rekursion zunächst die "am tiefsten" liegende Klammer gewandelt und dann immer weiter nach außen gegangen.

#### Weitere Dateien
Es gibt noch einige weitere Dateien im Projekt, die jedoch von Visual Studio automatisch instantiiert und dann nicht weiter von uns verändert wurden, sodass diese keinen Platz in dieser Dokumentation finden.

## Die Bot-Api
## Der Server