#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomizeCharacterOrientations))]

public class RandomizeCharacterOrientations_Inspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RandomizeCharacterOrientations randomizer = (RandomizeCharacterOrientations)target;

        if (GUILayout.Button("Randomize Orientations of Characters in the characterContainer"))
        {
            Transform[] children = randomizer.characterContainer.GetComponentsInChildren<Transform>();

            foreach (var child in children)
                if (child.IsInLayerMask(randomizer.characterMask))
                    child.rotation = RandomMaker.HorizontalRotation();
            

        }

    }

}
#endif

