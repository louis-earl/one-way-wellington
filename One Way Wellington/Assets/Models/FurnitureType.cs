using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FurnitureType
{
    public enum BuildCategory {Hull, Wall, Utility, Furniture, Staff, Loose};

    public string title;
    public string description;
    public BuildCategory category;
    public int cost;
    public int installTime;
    public int sizeX;
    public int sizeY;
    public bool fixedRotation;
    public bool requireElectricity;
    public bool requireFuel;
    public bool requireOxygen;
    public bool exteriorOnly;
    public bool multiSize;
    public bool walkable;
   


    public FurnitureType(string title, string description, BuildCategory category, 
        int cost, int installTime, int sizeX = 1, int sizeY = 1, bool fixedRotation = false, 
        bool requireElectricity = false, bool requireFuel = false, bool requireOxygen = false, 
        bool exteriorOnly = false, bool multiSize = false, bool walkable = true)
    {
        this.title = title;
        this.description = description;
        this.category = category;
        this.cost = cost;
        this.installTime = installTime;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.fixedRotation = fixedRotation;
        this.requireElectricity = requireElectricity;
        this.requireFuel = requireFuel;
        this.requireOxygen = requireOxygen;
        this.exteriorOnly = exteriorOnly;
        this.multiSize = multiSize;
        this.walkable = walkable;
    }

    public static Dictionary<string, FurnitureType> InstantiateFurnitureTypes()
    {
        Dictionary<string, FurnitureType> furnitureTypes = new Dictionary<string, FurnitureType>();

        /*
        furnitureTypes.Add("3D Printer", new FurnitureType(
            "3D Printer",
            "The 3D Printer can create new staff members and recover staff health.",
            FurnitureType.BuildCategory.Furniture,
            1000,
            5,
            sizeY: 2,
            requireElectricity: true
            ));
            */

        furnitureTypes.Add("Airlock", new FurnitureType(
            "Airlock",
            "Airlocks act as doors for the ship. Oxygen does not flow through when the Airlock is closed. Passengers, staff and enemies can open the Airlock, unless locked closed by the player.",
            FurnitureType.BuildCategory.Wall,
            200,
            4,
            requireElectricity: true,
            walkable: false
            ));

        /*
        furnitureTypes.Add("Art Station", new FurnitureType(
            "Art Station",
            "Passengers can sit at the workstation and create art. Passengers become happier and produce Artist Cards for the ship.",
            FurnitureType.BuildCategory.Furniture,
            150,
            3,
            sizeX: 2,
            requireElectricity: true));
            */

        furnitureTypes.Add("Battery", new FurnitureType("Battery",
            "Must be built inside. Stores electricity for your ship.",
            FurnitureType.BuildCategory.Utility,
            200,
            3,
            fixedRotation: true,
            multiSize: true
            ));

        furnitureTypes.Add("Bed", new FurnitureType(
            "Bed",
            "Passengers lay on the bed to sleep. The maximum number of passengers is determined by the number of beds on board.",
            FurnitureType.BuildCategory.Furniture,
            200,
            2,
            sizeX: 2
            ));

        /*
        furnitureTypes.Add("Cafe", new FurnitureType(
            "Cafe",
            "Cook staff stand at the Café and prepare food for passengers. Passengers will stand and queue while waiting for food.",
            FurnitureType.BuildCategory.Furniture,
            500,
            5,
            sizeX: 3,
            sizeY: 2,
            requireElectricity: true
            ));
            */

        furnitureTypes.Add("Charging Pad", new FurnitureType(
            "Charging Pad",
            "Staff use the charging pad when low on battery.",
            FurnitureType.BuildCategory.Furniture,
            300,
            5,
            sizeY: 2,
            requireElectricity: true
            ));

        /*
        furnitureTypes.Add("Couch", new FurnitureType(
            "Couch",
            "Passengers can sit on the couch. They can watch tv or rest.",
            FurnitureType.BuildCategory.Furniture,
            300,
            2,
            sizeX: 2
            ));
            */

        /*
        furnitureTypes.Add("Dining Table", new FurnitureType(
            "Dining Table",
            "Passengers sit at the dining table and eat.",
            FurnitureType.BuildCategory.Furniture,
            300,
            2,
            sizeX: 3,
            sizeY: 2
            ));
            */

        furnitureTypes.Add("Engine", new FurnitureType(
            "Engine",
            "A simple engine, nothing special. Must be built outside.",
            FurnitureType.BuildCategory.Utility,
            1000,
            10,
            sizeY: 2,
            fixedRotation: true,
            requireFuel: true,
            requireElectricity: true,
            exteriorOnly: true,
            walkable: false
            ));

        furnitureTypes.Add("Fuel Tank", new FurnitureType(
            "Fuel Tank",
            "Must be built outside. Stores fuel to allow your ship to travel further.",
            FurnitureType.BuildCategory.Utility,
            200,
            3,
            fixedRotation: true,
            exteriorOnly: true,
            multiSize: true
            ));

        /*
        furnitureTypes.Add("Hull", new FurnitureType(
            "Hull",
            "The base to any ship.",
            FurnitureType.BuildCategory.Hull,
            200,
            fixedRotation: true,
            exteriorOnly: true
            ));
        */

        furnitureTypes.Add("Oxygen Tank", new FurnitureType("Oxygen Tank",
            "Must be built inside. Stores oxygen to allow your passengers to breathe.",
            FurnitureType.BuildCategory.Utility,
            200,
            3,
            fixedRotation: true,
            multiSize: true
            ));

        furnitureTypes.Add("Oxygen Vent", new FurnitureType(
            "Oxygen Vent",
            "An Oxygen Vent fills a room with oxygen, so passengers and staff can breathe.",
            FurnitureType.BuildCategory.Utility,
            100,
            4,
            requireElectricity: true,
            requireOxygen: true
            ));
     
        furnitureTypes.Add("Ship Controls", new FurnitureType(
            "Ship Controls",
            "The captain sits at the ship controls.",
            FurnitureType.BuildCategory.Utility,
            500,
            5,
            sizeX: 1,
            sizeY: 2,
            fixedRotation: true,
            requireElectricity: true
            ));

        furnitureTypes.Add("Shower Stall", new FurnitureType(
            "Shower Stall",
            "Passengers can use the Shower Stall to increase their personal hygiene.",
            FurnitureType.BuildCategory.Furniture,
            300,
            4,
            sizeY: 2
            ));

        furnitureTypes.Add("Stairwell", new FurnitureType(
            "Stairwell",
            "Connect the interior and exterior of the ship.",
            FurnitureType.BuildCategory.Utility,
            200,
            5,
            sizeX: 2,
            fixedRotation: true
            ));

        /*
        furnitureTypes.Add("Solar Panel", new FurnitureType(
            "Solar Panel",
            "Solar panels collect electricity for the ship.",
            FurnitureType.BuildCategory.Utility,
            200,
            4,
            fixedRotation: true,
            requireElectricity: true,
            exteriorOnly: true
            ));
            */

        /*
        furnitureTypes.Add("Support Beam", new FurnitureType(
            "Support Beam",
            "Used for connecting exterior structures",
            FurnitureType.BuildCategory.Hull,
            100,
            3,
            exteriorOnly: true
            ));
            */

        furnitureTypes.Add("Toilet Stall", new FurnitureType(
            "Toilet Stall",
            "Passengers can relieve their bladders in a Toilet Stall.",
            FurnitureType.BuildCategory.Furniture,
            300,
            4,
            sizeY: 2
            ));

        // Turrets not planned for release 

        /*

        furnitureTypes.Add("Turret", new FurnitureType(
            "Turret",
            "Turrets are used to damage enemy ships. They must be connected to a turret control system to function.",
            FurnitureType.BuildCategory.Furniture,
            400,
            fixedRotation: true,
            requireElectricity: true,
            exteriorOnly: true
            ));

        furnitureTypes.Add("Turret Controls", new FurnitureType(
            "Turret Controls",
            "Turrets are linked to the closest available turret control system. Guards sit at the turret control system and operate the turret.",
            FurnitureType.BuildCategory.Furniture,
            200,
            requireElectricity: true
            ));

        */

        /*
        furnitureTypes.Add("TV", new FurnitureType(
            "TV",
            "Passengers become happier when watching TV.",
            FurnitureType.BuildCategory.Furniture,
            300,
            4,
            sizeX: 2,
            requireElectricity: true
            ));
            */

        
        furnitureTypes.Add("Wall", new FurnitureType(
            "Wall",
            "If you can read this, something isn't right.",
            FurnitureType.BuildCategory.Loose,
            100,
            3,
            fixedRotation: true,
            multiSize: true,
            walkable: false
            ));
            






        // LOOSE ITEMS 
        furnitureTypes.Add("Cargo", new FurnitureType(
            "Cargo",
            "If you can read this, something isn't right",
            FurnitureType.BuildCategory.Loose,
            0,
            0,
            fixedRotation: true
            ));

        return furnitureTypes;
    }


}
