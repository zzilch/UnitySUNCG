# UnitySUNCG
A simple SUNCG viewer and physics simulator based on Unity, tested in Unity 2018.3

### Dependency
- [Runtime OBJ Importer](https://assetstore.unity.com/packages/tools/modeling/runtime-obj-importer-49547): runtime obj model reader, free
#### Optional
 - [NonConvexMeshCollider](https://assetstore.unity.com/packages/tools/physics/non-convex-mesh-collider-84867): get better colliders but spend more time, paid

### File Organization
```shell
UnitySUNCG
    |-- Assets
        |-- UnitySUNCG
            |-- Config.cs - Environment settings
            |-- Json.cs - Data structure for SUNCG
            |-- Scene.cs - Json loader and gameobjects creator
            |-- Loader.cs - Meshes, materials and colliders loader
            |-- Utils.cs - Useful functions
            |-- Samples
                |-- SampleHouse.cs - A sample for loading a whole house
                |-- SampleRoom.cs - A sample for loading a single room
            |-- pose
                |-- object - Human poses models with parts
                |-- material - material for pose models
        |--OBJImport
```

### Usage
1. Download SUNCG dataset
2. Set `$Config.SUNCG_HOME$` as path to SUNCG

#### Load a whole house  
See scene SampleHouse in UnityEditor
```c#
    string houseId = "000d939dc2257995adcb27483b04ad04";    // Provide a house id

    House house = Scene.GetHouseById(houseId);              // Read the house json
    GameObject houseObject = Scene.GetHouseObject(house);   // Create gameobjects
    // Read the json file and create gameobjects in one line
    // GameObject houseObject =  GetHouseObjectById(string houseId)

    houseObject.GetComponent<HouseLoader>().Load();         // Load meshes
    // houseObject.GetComponent<HouseLoader>().Load(true);  // Use left hand coord
```
#### Load a single room  
See scene SampleRoom in UnityEditor
```c#
    string fullId = "35c7af9c459e7c96920f81ae1b16f3aa_0_0";             // Provide a room full id
    GameObject houseObject = Scene.GetRoomHouseObjectById(fullId);      // Read the house json and create gameobjects
    // Read the json file and create gameobjects respectively
    // House house = GetHouseById(fullId.Split('_')[0]);
    // GameObject houseObject = Scene.GetRoomHouseObject(house, 0, 0);

    houseObject.GetComponent<HouseLoader>().Load(true);                 // Load meshes
```