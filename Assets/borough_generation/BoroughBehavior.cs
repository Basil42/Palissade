using System;
using System.Collections.Generic;
using UnityEngine;

namespace borough_generation
{
    public class BoroughBehavior : MonoBehaviour
    {
        public Node Tile;
        public List<GameObject> elements;

        private void Start()
        {
            RaycastHit hit;
            for (int i = 0; i < elements.Count; i++)
            {
                var elementTrans = elements[i].transform;

                if (Physics.Raycast(elementTrans.position, Vector3.down, out hit))//TODO: multiple ray cast to average the normal so "cliff houses" don't fuck up too hard
                {
                    elementTrans.position = hit.point + new Vector3(0f,elements[i].GetComponent<MeshRenderer>().bounds.extents.y,0f);//not ideal but okay
                        //elementTrans.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
                
            }
        }
    }
}