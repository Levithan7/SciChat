# Dokumentation von SciChat

## Das Chatprogramm
### Nutzung von SciChat als regulärer Nutzer
### Speichern der Daten
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
Deklariert man das Propertiy eines Model als SQLProperty so besteht dabei die Option den Variablennamen ungleich den Namen der Spalte in der Tabelle zusetzen.
BEISPIEL EINFÜGEN
Diese Methode gibt die Werte dieser Properties zurück.

**string? ModifyProperty(dynamic? input)**
Diese Methode wandelt den Wert eines Properties so um, dass dieser in SQL verwendbar ist.
Zahlen bleiben dabei gleich. Um Strings jedoch werden Anführungszeichen gesetzt.

**enum ChangeType**
Dieser enum deklariert alle Änderungstypen. Aktuell sind nur Insert und Update vorhanden wobei nur für Insert eine Funktionalität implementiert wurde.

## Die Bot-Api
## Der Server
