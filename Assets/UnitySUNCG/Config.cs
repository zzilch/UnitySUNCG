using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySUNCG
{
    public class Config
    {
        public static string SUNCG_HOME = "path/to/SUNCG"; // Todo: set the path to SUNCG
        public static string PART_POSE_HOME = Application.dataPath+"/UnitySUNCG/pose/";
        public static bool SHOW_WALL = true, SHOW_FLOOR = true, SHOW_CEILING = false, USE_PART_POSE = true;
        public static bool USE_LEFT_HAND = false;
        public static string[] POSE_IDS = {"325","346","324","323","333"};
    }

}
