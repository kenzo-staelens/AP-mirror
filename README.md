# WPF Maze generator

## projectstructuur
- Components
	- bevatten data stores die aan datastructuren kunnen worden meegegeven via de constructor enkel als deze nodig zijn
	- IComponent (base interface)
	- MazeConstructionComponent (bevat data nuttig voor generators)
	- WallDataComponent (nuttig voor physics en rendering)
	- GeneratorRequirementComponent (bevat extra datastore dat generators dynamisch kunnen aanvullen in de vorm van een dictionary)
- Datalaag
	- klassen en interfaces voor interactie met externe data
	- FileManager (uitlezen van bestanden)
- ExtensionMethods & UtilityFunctions
	- self explanatory 
- [Generators](Generators)
	- interfaces & klassen voor maze generators en maze generator factory
	- IMazeGenerator
	- IMazeGeneratorFactory
	- MazeGeneratorFactory
	- StaticGenerator
	- RecursiveBacktrackingGenerator
	- RecursiveDivisionGenerator
- [Globals](Datastructuren)
	- bevat datastructuren (kan dus ook Models genoemd worden)
	- Maze
	- Cell
	- Ball
	- enum MazeGeneratorTypes (gebruikt in factory)
- Runnable Projecten
	- Console_Test_Environment
	- WPF_maze_generator
	- WPF_physics_simulator (leeg)

## Datastructuren
maze is een simpele collectie van cellen. Voor interne cellen wordt verder ook hun buren aangevuld (zie hieronder `Cell`)
```
Maze
	- readonly int Width, Height
	- readonly Cell[,] cells
```

Cell bevat velden voor Neighbours voor gebruik bij het aanpassen van muren.
Zo kan als bij een cel de rechtermuur wordt aangepast in de rechtercel meteen de linkermuur aangepast worden(zie verder [muren updaten](#muren-updaten));  
Bovendien is het ook sneller om een graph te maken omdat vanuit cell een referentie is naar de gevraagde naburige cel. \
cellen kunnen optioneel ook andere componenten bevatten die gebruikt worden door andere systemen (rendering, physics, genereren, etc.);
Deze worden via de constructor megegeven
```
Cell
	- readonly int X,Y /*index of cell*/
	- Cell[4] Neighbours
	- readonly bool[4] walls
	- private readonly IComponent[]
	- public IComponent? GetComponent(Type<IComponent> ComponentType)
```

voor later gebruik horen X, Y aanpasbaar te zijn, grootte van de bal mag wel constant blijven
```
Ball
	- int X, Y
	- readonly int Size
	- private readonly IComponent[]
	- public IComponent? GetComponent(Type<IComponent> ComponentType)
```

### muren updaten

in cellen zit een methode `SetWall(int index, bool isSet, optional bool cascade=true)`  
met deze methode kan de cel zijn muren aanpassen met `walls[index] = isSet`;
Moest cascade `true` zijn zal ook de cel aan de overkant van de muur(`Neighbours[(index+2)%4]`) aangepast worden maar zal verder geen cascade meer gebeuren.

## Generators

### StaticGenerator
de statische generator maakt gebruik van een string representatie van muren per cell met volgende structuur  
(1: muur bestaat; 0: muur bestaat niet)
```
maze_breedte maze_hoogte  
muur_boven muur_rechts muur_onder muur_links   
...  
```
waarbij bestaande muren niet bestaande muren overschrijven; muren worden van links naar rechts ingevuld;
er mogen meer muren gedefinieerd zijn dan de gegeven grootte (`breedte*hoogte`) maar deze worden genegeerd
zo is bijvoorbeeld de configuratie
```
2 2
1 0 0 1
1 1 0 0
1 0 1 1
0 1 1 _
```
de definitie voor volgend doolhof ( _ mag beiden 0 of 1 bevatten)
```
.--.--.
|     |
.  .  .
|  |  |
.--.--.
```

#### Technische keuzes
- er werdt gekozen om lege muren niet te plaatsen zodat dit een additief algoritme wordt;
  Zo zijn de verwachtingen van string data ook iets laxer in geval de gebruiker een fout maakt in die data.


### Recursive Backtracking Generator

#### Algoritme

- kies een willekeurige cel
- markeer de cel als bezocht, dit is je startpunt
- zolang je niet bezochte cellen bestaan
  - kies een niet bezochte buur
  - maak een gang tussen de huidige cel en die buur
  - markeer de buur als bezocht
  - de huidige cel wordt de net gekozen buur
  - als geen buren bestaan keer terug naar de vorige cel
- wanneer geen onbezochte cellen bestaan is het doolhof gemaakt

#### Technische keuzes
- Er is gebruik gemaakt van een `Stack<Cell>` in plaats van recursie om geen (of in ieder geval een veel hoger)
 limiet van celdiepte te hebben aangezien dit algoritme vrij diepe recursie kan genereren (worst case = O(n*m))

- Er wordt gebruik gemaakt van de GeneratorRequirementComponent als data store om bij te houden of de cel al bezocht geweest is
  er is voor deze manier gekozen omdat niet elke generator dit in een cel moet markeren en een cel dus geen veld voorziet om deze data bij te houden

- Voor het kiezen van een geldige buur wordt van linq gebruik gemaakt vanwege deze implementatie moet bij het aanpassen van muren eerst bij
  de huidige cel de index van de buur gezocht worden om de index van de muur te bepalen; Door het cascade mechanisme van setWall moet die functie enkel op de huidige cel toegepast worden

### Recursive Division Generator

#### Algoritme
- selecteer de eerste sectie als het volledige doolhof
- zolang deelbare secties bestaan (minumum 6 cellen groot of met vorm 2x2)
  - deel de zone horizontaal of verticaal
  - plaats een muur langs de volledige deellijn
  - maak een gat ergens in de net gebouwde deellijn
  - herhaal voor elk van de nieuwe gegenereerde (kleinere) secties
- optioneel: bouw buitenmuren

#### Technische keuzes

- net zoals de vorige generator is hier ook van een Stack gebruik gemaakt;
  In dit geval is het echter `Stack<int[4]>` waar elke `int[4]` de index en van de linkerbovenhoek en rechteronderhoek van een sectie bijhoudt.
- secties van 2x2 worden alsnog verwerkt omdat anders een cirkel ontstaat (ga steeds links of rechts in dit vierkant)
- secties met breedte of hoogte 1 worden overgeslaan aangezien 1 van de richtingen niet deelbaar is en in de andere de geplaatste muur toch weer meteen wordt weggehaald,
  dit is dus een nuloperatie en vraagt vervolgens meer resources dan nodig zijn (de gedeelde secties worden anders ook aan de stack toegevoegd en creëren dan nog meer nuloperaties)
- er werdt gekozen voor secties met minimum 6 cellen omdat een sectie met 5 (of minder; uitzondering bij 2x2) cellen enkel kan delen in (2x1; 3x1) secties
  waarbij 2x1 een te kleine sectie is om toe te laten
- in plaats van 50/50 willekeurig te kiezen voor de richting van delen wordt `bool horizontaal = RandomDouble()>itemWidth / (itemHeight + itemWidth`
  gebruikt, bij 50/50 lijkt namelijk een bias te zijn voor lange smalle gangen, met deze formule wordt de bias naar eerder vierkantige secties geduwd

## Grafische Applicatie

in de grafische applicatie zijn velden voorzien voor
- het selecteren van generator
- breedte en hoogte van het doolhof (default 11) (enkel non static generator)
- het selecteren van een tekstbestand (default = "./default.txt") (enkel static generator)

Er is ook een Canvas(550x550) aanwezig waar het doolhof op gerenderd wordt,
de grootte van de getekende cellen wordt dynamisch aangepast aan de grootte van het doolhof `breedte/#cellen`

Als errors gegenereerd worden komen die onder de "Generate" knop