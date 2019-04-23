using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySUNCG;

public class SampleHouse : MonoBehaviour
{
    public House house;
    public GameObject houseObject;
    // Start is called before the first frame update
    void Start()
    {
        string houseId = "000d939dc2257995adcb27483b04ad04";
        house = Scene.GetHouseById(houseId);
        houseObject = Scene.GetHouseObject(house);
        houseObject.GetComponent<HouseLoader>().Load(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
