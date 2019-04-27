using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Dummiesman;
using System;

namespace UnitySUNCG
{

    public class HouseLoader : MonoBehaviour
    {
        public House house;

        /// load meshes in a house, left hand coordinate system by default
        public void Load()
        {
            foreach (Transform levelTransform in transform)
                levelTransform.GetComponent<LevelLoader>().Load();
            // convert Unity coordinate system to right hand coordinate system
            if (Config.USE_LEFT_HAND)
                transform.localScale = new Vector3(1, 1, -1);
        }
    }


    public class LevelLoader : MonoBehaviour
    {
        public Level level;

        /// load meshes in a level
        public void Load()
        {
            foreach (Transform nodeTransform in transform)
            {
                NodeLoader nodeLoader = nodeTransform.GetComponent<NodeLoader>();
                // Nodes in a room will be loaded in the room
                if (nodeLoader.node.roomId != null)
                    continue;
                nodeTransform.GetComponent<NodeLoader>().Load();
            }

        }
    }

    public class NodeLoader : MonoBehaviour
    {
        public Node node;

        /// Load external defined materials
        private static UnityEngine.Material LoadMaterial(Material material)
        {
            UnityEngine.Material mat = new UnityEngine.Material(Shader.Find("Standard")) { name = material.name };
            if (material.diffuse != null)
            {
                Color c;
                ColorUtility.TryParseHtmlString(material.diffuse, out c);
                mat.color = c;
            }
            if (material.texture != null)
            {
                string texturePath = Config.SUNCG_HOME + "texture/" + material.texture + ".jpg";
                Texture tex = ImageLoader.LoadTexture(texturePath);
                mat.mainTexture = tex;
            }
            return mat;
        }

        /// Load the object mesh for this gameobject
        private void LoadMesh()
        {
            if (Config.USE_PART_POSE && Array.IndexOf(Config.POSE_IDS, node.modelId) > -1)
            {
                string poseType;
                if (node.modelId == "323" || node.modelId == "333")
                    poseType = "Sitting";
                else if (node.modelId == "324")
                    poseType = "Standing";
                else
                    poseType = "Laying";

                // Instatiate the ybot pose perfab
                GameObject loadedObject = (GameObject)Instantiate(Resources.Load("ybot@" + poseType));
                loadedObject.transform.parent = transform;

                // Apply the transform of the node
                if (node.transform != null)
                {
                    Matrix4x4 matrix = TransformUtils.Array2Matrix4x4(node.transform);
                    TransformUtils.SetTransformFromMatrix(transform, ref matrix);
                }

                // Slight rise up the model to refine the pose
                Transform root = loadedObject.transform.Find("mixamorig:Hips");
                root.position = new Vector3(root.position.x, root.position.y + 0.5F, root.position.z);
                return;
            }
            else
            {
                string pathToObj = Config.SUNCG_HOME;
                pathToObj += "object/" + node.modelId + "/" + node.modelId + ".obj";
                if (File.Exists(pathToObj))
                {
                    // Load the mesh with an empty gameobject as parent
                    GameObject loadedObject = new OBJLoader().Load(pathToObj);
                    List<Transform> children = TransformUtils.GetChildrenList(loadedObject.transform);

                    foreach (Transform child in children)
                    {
                        // Add a MeshCollder to every part of the model
                        child.gameObject.AddComponent<MeshCollider>();

                        // Load external materials
                        if (node.materials.Length > 0)
                        {
                            MeshRenderer mr = child.GetComponent<MeshRenderer>();
                            UnityEngine.Material[] materials = new UnityEngine.Material[Math.Max(node.materials.Length, mr.materials.Length)];
                            for (int i = 0; i < materials.Length; i++)
                            {
                                if (i < node.materials.Length &&
                                (node.materials[i].diffuse != null || node.materials[i].texture != null))
                                    materials[i] = LoadMaterial(node.materials[i]);
                                else
                                    materials[i] = mr.materials[i];
                            }
                            mr.materials = materials;
                        }

                        // Change the parent of the mesh to this gameobject
                        child.parent = transform;
                    }
                    Destroy(loadedObject);

                    // Apply the transform of the node
                    if (node.transform != null)
                    {
                        Matrix4x4 matrix = TransformUtils.Array2Matrix4x4(node.transform);
                        TransformUtils.SetTransformFromMatrix(transform, ref matrix);
                    }

                    // Add rigid body to the gameobject and fix the rotation
                    Rigidbody rigid = gameObject.AddComponent<Rigidbody>();
                    rigid.isKinematic = true;
                    rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                }
                else
                {
                    Debug.Log("Can't find object " + pathToObj);
                }
            }

        }

        /// Load the room mesh for this room gameobject
        private void LoadRoomMesh(string type, string suffix)
        {
            string houseId = transform.root.GetComponent<HouseLoader>().house.id;
            string pathToObj = Config.SUNCG_HOME + "room/" + houseId + "/" + node.modelId + suffix + ".obj";
            if (File.Exists(pathToObj))
            {
                GameObject roomObject = new OBJLoader().Load(pathToObj);
                roomObject.name = type + "#" + node.id;
                roomObject.transform.parent = transform;
                List<Transform> children = TransformUtils.GetChildrenList(roomObject.transform);

                // Ground: the gameobject has created by GetNodeObject()
                // W/F/C: add the empty parent gameobject to this room 
                foreach (Transform child in children)
                {
                    child.gameObject.AddComponent<BoxCollider>();
                    child.parent = (type == "Ground" ? transform : roomObject.transform);
                }

                // Apply the transform of the node
                if (node.transform != null)
                {
                    Matrix4x4 matrix = TransformUtils.Array2Matrix4x4(node.transform);
                    TransformUtils.SetTransformFromMatrix(transform, ref matrix);
                }
            }
        }

        public void Load()
        {
            if (node.type == "Room")
            {
                List<Transform> children = TransformUtils.GetChildrenList(transform);
                foreach (Transform child in children)
                {
                    child.GetComponent<NodeLoader>().Load();
                }

                if (Config.SHOW_WALL)
                    LoadRoomMesh("Wall", "w");
                if (Config.SHOW_FLOOR)
                    LoadRoomMesh("Floor", "f");
                if (Config.SHOW_CEILING)
                    LoadRoomMesh("Ceiling", "c");
            }
            else if (node.type == "Ground")
                LoadRoomMesh("Ground", "f");
            else if (node.type == "Object")
                LoadMesh();
        }

    }

}
