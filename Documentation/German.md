# Dokumentation von SciChat

## Nutzung von SciChat als regulärer Nutzer

## Nutzung von SciChat als Bot-Developer
In SciChat ist es möglich einen Nutzer zu erstellen, den mal per C#-Programmcode automatisieren kann. Ein solcher automatisierter Nutzer wird ab jetzt als `Bot` bezeichnet. Um  einen Bot zu erstellen ist [https://github.com/Levithan7/SciChat/tree/main/SciChatApi](Die BotAPI) zu klonen. Dann lässt sich in der `Program.cs` eine Instanz der `Bot`-Klasse erstellen und darüber Handeln. Ein Beispiel dafür ist [https://github.com/Levithan7/SciChat/tree/main/SciChatApi](hier) beflindlich. Die restlichen Dateien sind nicht zu modifizieren.

## Die Entwicklung
Zur Entwicklung von SciChat haben wir folgende Software verwendet:
- Visual Studio 2022 -> C# Entwicklungsumgebung
- SQL Server Managment Studio -> SQL
- Github -> Verwalten des Codes

## Der Quellcode

### SciChatProjekt
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

### Die Bot-Api
Die Bot-Api kann verwendet werden, um Computergesteuerte User zu entwickeln bspw. zur Ausführung erstellter Commands. Die API ist nicht sehr ausgearbeitet, da sie nur einen nebensächlichen Teil des Projektes darstellt. Trotzdem sollte sie ausreichen, um grundlegende Funktionalität eines Bots (vergleichbar mit älteren Discord Bots) zu implementieren.
Die Bot-Api ist dabei per Github zu klonen. In einer Datei `Program.cs` kann der entsprechende Bot-Developer dann seinen Bot programmieren.
In der vorgefertigten `Bot.cs` liegt die Vorlage für einen Bot. Dabei ist zu beachten, dass der Bot niemals direkt auf die Datenbank zugreift sondern stattdessen einen Request an
den Server schickt, der dann prüft, ob die korrekten Anmeldeinformationen für den Bot vorliegen. So kann die Sicherheit der Daten garantiert werden.

#### Models
Hierbei handelt es sich letzlich um eine Kopie der Models des `SciChatProjects`, die jedoch nicht über die entsprechenden Methoden verfügen sondern nur um die Attribute. Das wird dafür benötigt, dass die Rückgegebenen Daten des Servers in Objekte gewandelt werden können.

#### Constants.cs
In dieser Datei ist - stand jetzt - eine einzige Konstante definiert. Dabei handelt es sich um die url der API. Weitere Konstanten können hier hinzugefügt werden.

#### Program.cs
Das ist ein Beispiel für die Umsetzung eines Bots. Dieser Bot hat die `id=1` und das `Password="123"`.
Zum start des Bots werden erstmal alle Nachrichten abgerufen. Anschließend werden in alle Konversationen, in denen der Bot ist, die Nachricht 'Bot started!' gesendet. Danach begibt sich der Bot in eine unendlich lange Schleife, in der zunächst die neuen Nachrichten geladen werden und dann per `Regex` geprüft wird, ob der Command `/forloop <anzahl>` aufgerufen wird. Über diesen Command wird in die Konversation, in der der Command aufgerufen wurde eine Nachricht mit allen Zahlen von `0` bis `anzahl-1` gesendet.
Anschließend schläft der Bot für eine Sekunde, damit die API nicht überlastet wird.

#### Bot.cs
**List<Message> GetSentMessage()**

In dieser Methode werden alle Nachrichten abgerufen, die der Bot jemals gesendet hat.

**List<Message> GetSentReceivedMessages(bool update = false)**

Diese Methode gibt alle Nachrichten zurück, der der User jemals erhalten hat.
Wird update dabei auf `true` gesetzt, so werden die Nachricht für der Rückgabe nocheinmal per
Server Request aktualisiert.

**List<Conversation>? GetConversations()**

Gibt eine Liste mit allen Conversations zurück. TODO: Ist er in keiner Conversation soll eine leere Liste
statt null zurückgegeben werden.

**void SendMessage(string content, int convID)**

Diese Methode lässt den Bot eine Nachricht senden.

**List<Message> UpdateReceivedMessages()**

Dies Methode gibt alle Nachrichten zurück, die der Bot seit der letzten Aktualisierung erhalten hat.

**string FetchViaParam(string suburl, Dictionary<string, string> parameters, bool addCredentials = true)**

Diese Methode ruft `static string FetchViaParam(string suburl, Dictionary<string, string> parameters)` auf, ist selbst aber nicht statisch und bietet über `addCredentials=true` die Möglichkeit, die Anmeldedaten für die API (also die Nutzerid sowie das Passwort des Nutzers) zu den Parametern hinzuzufügen. Das ist notwendig, um einen Fetch auf den Server durchzuführen.

**static string FetchViaParam(string suburl, Dictionary<string, string> parameters)**
Diese Method ruft `static string FetchData(string request)`, und erstellt dazu aus dem Parameter-Dictionary sowie der suburl (also dem Namen des Request; z.B. 'userReceivedMessagesByID') einen entsprechenden Request.

**static string FetchData(string request)**
Diese Methode wird im allgemeinen verwendet, um Get-Requests an den Server zu schicken und gibt die entsprechenden Werte zurück.
TODO null-Rückgabe bei Fehler!

**void PostData(string url, Dictionary<string, string> parameters, bool addCredentials = true)**
Diese Method ruft `static void PostData(string url, Dictionary<string, string> parameters)`, und erstellt dazu aus dem Parameter-Dictionary sowie der suburl (also dem Namen des Request; z.B. 'addmessage') einen entsprechenden Request.

**static void PostData(string url, Dictionary<string, string> parameters)**
Diese Methode wird im allgemeinen verwendet, um Post-Requests an den Server zu schicken.
Dabei wird zunächst ein entsprechender Query über die angegebenen Paramter erstellt.

**Dictionary<string, string> AddCredentials(Dictionary<string, string> param, bool addCredentials)**
Diese Methode fügt die id, sowie das Passwort des Bots zu einem Parameter-Dictionary hinzu.

### Der Server
Der `Server` ist das dritte und als letztes erstellte Unterprojekt von `SciChat`.
Wie bereits beschrieben wird es primär verwendet, um Requests des Bots abzufragen. Der Server ist in Bezug auf Passwort Implementation noch nicht fertig entwickelt.
Wichtig ist dieses Projekt nicht mit dem letzendlichen Server zu verwechseln auf dem das Projekt gehostet wird. Dieser wird als `Host` bezeichnet.  

#### Controllers.ServerController.cs
Das ist die einzige modifizierte Datei, die nicht zu 100% von Visual Studio erstellt wurde. Der Rest sind Projektdateien. Wie auch im Code dokumentiert, erwartet jede Methode das Feld `u` (ID des Users) sowie `p` (Passwort des User). Alle Methoden haben das Attribut `HttpHet(string name)` bzw. `HttpPost(string name)`. Dabei ist `name` der 'Command', der letzendlich beim Aufruf der API verwendet wird. Alle Methoden geben ein `IActionResult` zurück. Das ist dann vereinfacht ausgedrückt der Status, der aus der Abfrage resultiert (z.B. `200, Ok` oder `403, Frobidden` usw.). In jeder Methode wird zunächst geprüft, ob
der Nutzer, der versucht einen Request zu erstellen dazu berechtigt ist. Wenn nicht wird `Unauthorized` zurückgegeben.

**IActionResult GetConversation(int id, int u, string p)**

Gibt die Konversation der ID `id` zurück. Allerdings nur, wenn der Nutzer, der die Abfrage ausführt auch Teil dieser Konversation ist.
TODO: SICHERHEIT

**IActionResult GetConversationsByUserID(int userid, int u, string p)**

Gibt alle Konversationen zurück, in denen sich der User der id 'userid' befindet. Allerdings nur, wenn
der User, der den Request ausführt auch der User ist, dessen Konversationen ausgegeben werden sollen.
TODO: SICHERHEIT

**IActionResult GetConversationMembers(int id, int u, string p)**

Gibt alle Mitglieder einer Konversation der ID `id` zurück, wenn sich der Nutzer, der das Request erstellt in dieser Konversation befindet.
TODO: SICHERHEIT

**IActionResult GetConversationMessages(int id, int u, string p)**

Gibt alle Nachrichten einer Konversation der ID `id` zurück, wenn sich der Nutzer, der das Request erstellt in dieser Konversation befindet.
TODO: SICHERHEIT

**IActionResult GetUserReceivedMessages(int userid, int u, string p, bool includeOwn=false)**

Gibt alle Nachrrichten zurück, die der Nutzer der ID `userid` jemals erhalten hat. Ist `includeOwn` auf `true` gesetzt, werden dabei auch Nachrichten berücksichtigt, die der Nutzer selbst geschickt hat. Allerdings nur, wenn der Nutzer, der den Request erstellt hat auch der Nutzer ist, der den Request erstellt hat.
TODO: Sicherheit

**IActionResult GetUserSentMessages(int userid, int u, string p)**
Gibt alle Nachrrichten zurück, die der Nutzer der ID `userid` jemals gesendet hat. Allerdings nur, wenn der Nutzer, der den Request erstellt hat auch der Nutzer ist, der den Request erstellt hat.
TODO: Sicherheit

**IActionResult AddMessage(int userid, int conversationid, string content, int u, string p)**
Lässt den Nutzer der ID `userid` eine Nachricht des Inhalts `content` in die Konversation der ID `conversationid` schicken, wenn der Nutzer
- dem Nutzer entsprecht, der den Request ersetllt hat UND
- der Nutzer Teil der Konversation ist
