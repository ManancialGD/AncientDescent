using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AncientDescent.Puzzles.SecondFloor
{
    public class SecondFloorPuzzle : MonoBehaviour
    {
        [SerializeField] private Face[] faces;
        [SerializeField] private Sprite[] order;
        [SerializeField] private UnityEvent onPuzzleSolved;

        private readonly Dictionary<int, Sprite> currentFaces = new();

        private void Start()
        {
            for (int i = 0; i < faces.Length; i++)
            {
                faces[i].Initialize(i, order);
                faces[i].FaceChanged += CheckSolution;
            }
        }

        private void OnDestroy()
        {
            foreach (Face face in faces)
            {
                face.FaceChanged -= CheckSolution;
            }
        }

        private void CheckSolution(int index, Sprite changedFace)
        {
            currentFaces[index] = changedFace;
            
            foreach (Sprite face in order)
            {
                if (!currentFaces.ContainsValue(face))
                    return;
            }

            onPuzzleSolved.Invoke();
        }
    }
}