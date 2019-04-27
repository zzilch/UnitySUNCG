using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySUNCG;

public class SampleRoom : MonoBehaviour
{
    public House house;
    public GameObject houseObject;
    // Start is called before the first frame update
    void Start()
    {
        //string fullId = "7859961f4b61112f773b224b3fc2cccd_0_1"; // room with standing, sitting, laying pose
        string fullId = "35c7af9c459e7c96920f81ae1b16f3aa_0_0";
        houseObject = Scene.GetRoomHouseObjectById(fullId);
        HouseLoader houseLoader = houseObject.GetComponent<HouseLoader>();
        houseLoader.Load();
    }

    // Update is called once per framed
    void Update()
    {
        
    }
}
