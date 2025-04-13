#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(BodyParts))]
public class BodyParts_Inspector : Editor
{
    Vector3 leftLocalPosition = new Vector3(-0.123f,-0.0335f,0.056f);
    Vector3 leftLocalEulerAngles = new Vector3(21.2f,1f,172f);
    Vector3 leftLocalScale = new Vector3(1,1,1);
    Vector3 rightLocalPosition = new Vector3(0.134f,-0.023f,-0.005f);
    Vector3 rightLocalEulerAngles = new Vector3(0.004f,20.01f,359.98f);
    Vector3 rightLocalScale = new Vector3(1,1,1);


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BodyParts bodyParts = (BodyParts)target;
        Ragdoll ragdoll= bodyParts.GetComponentInSiblings<Ragdoll>(false);

        if (GUILayout.Button("Create toolHolders and get references on model and ragdoll"))
        {
            bodyParts.leftHand = CreateToolHolder(bodyParts.transform, Hand.left);
            bodyParts.rightHand = CreateToolHolder(bodyParts.transform, Hand.right);
            ragdoll.animatedHips = GetChildByName(bodyParts.transform, "Hips", "pelvis");
            ragdoll.ragdollLeftHand = CreateToolHolder(ragdoll.transform, Hand.left);
            ragdoll.ragdollRightHand = CreateToolHolder(ragdoll.transform, Hand.right);
            ragdoll.ragdollHips = GetChildByName(ragdoll.transform, "Hips", "pelvis");
            EditorSceneManager.MarkSceneDirty(bodyParts.gameObject.scene);
        }

        if (GUILayout.Button("Get references to toolHolders and hips on model and ragdoll"))
        {
            bodyParts.leftHand = GetChildByName(bodyParts.transform, "LeftToolHolder");
            bodyParts.rightHand = GetChildByName(bodyParts.transform, "RightToolHolder");
            ragdoll.animatedHips = GetChildByName(bodyParts.transform, "Hips", "pelvis");
            ragdoll.ragdollLeftHand = GetChildByName(ragdoll.transform, "LeftToolHolder");
            ragdoll.ragdollRightHand = GetChildByName(ragdoll.transform, "RightToolHolder");
            ragdoll.ragdollHips = GetChildByName(ragdoll.transform, "Hips", "pelvis");
            EditorSceneManager.MarkSceneDirty(bodyParts.gameObject.scene);
        }

        if (GUILayout.Button("UnEquip Weapons"))
        {
            UnEquipWeapons(bodyParts);
            EditorSceneManager.MarkSceneDirty(bodyParts.gameObject.scene);
        }
        if (GUILayout.Button("Equip Weapons"))
        {
            EquipWeapons(bodyParts);
            EditorSceneManager.MarkSceneDirty(bodyParts.gameObject.scene);
        }



        /*
        if (GUILayout.Button("Standardize names in Rig\n(Provided that the transform hierachy matches completely)"))
            StandardizeNaming_IfEntireHierachyMatches(bodyParts.RigToBeRenamed, bodyParts.correctlyNamedRig);

        if (GUILayout.Button("Standardize names in Rig\n(Use whatever subset of the transform hierachy that matches)"))
            StandardizeNaming_WhenEverPossible(bodyParts.RigToBeRenamed, bodyParts.correctlyNamedRig);
        */
    }


    Transform CreateToolHolder(Transform model, Hand hand)
    {
        string handedness = (hand == Hand.right) ? "Right" : "Left";
        string altHandName = (hand == Hand.right) ? "hand_r" : "hand_l";
        Transform handObject = GetChildByName(model.transform, handedness + "Hand", altHandName);

        Transform toolHolder = handObject.Find(handedness + "ToolHolder");
        if (toolHolder == null)
        {
            toolHolder = new GameObject(handedness + "ToolHolder").transform;
            toolHolder.SetParent(handObject);
            SetTransform(toolHolder, hand);
        }
        return toolHolder;
    }

    Transform GetChildByName(Transform transform_, string name, string alternativeName = null)
    {
        Transform[] children = transform_.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (Transform child in children)
            if (child.name.ToLower() == name.ToLower())
                return child;

        if (alternativeName != null)
            return GetChildByName(transform_, alternativeName);
        if (!transform_.GetChild(0).gameObject.activeSelf)
            Debug.LogWarning($"{transform_} aren't active. Therefore we cannot find any children on it");
        else
            Debug.LogWarning($"didn't find child named {name} on {transform_}");
        return null;
    }

    void SetTransform(Transform transform_, Hand hand)
    {
        if (hand == Hand.left)
        {
            transform_.localPosition = leftLocalPosition;
            transform_.localEulerAngles = leftLocalEulerAngles;
            transform_.localScale = leftLocalScale;
        }
        else
        {
            transform_.localPosition = rightLocalPosition;
            transform_.localEulerAngles = rightLocalEulerAngles;
            transform_.localScale = rightLocalScale;
        }
    }

    void EquipWeapons(BodyParts bodyParts)
    {
        UnEquipWeapons(bodyParts);

        if (bodyParts.leftWeapon != null)
            Instantiate(bodyParts.leftWeapon, bodyParts.leftHand);
        if (bodyParts.rightWeapon != null)
            Instantiate(bodyParts.rightWeapon, bodyParts.rightHand);
    }

    void UnEquipWeapons(BodyParts bodyParts)
    {
        for (int i = bodyParts.leftHand.childCount; i > 0; --i)
            DestroyImmediate(bodyParts.leftHand.GetChild(0).gameObject);
        for (int i = bodyParts.rightHand.childCount; i > 0; --i)
            DestroyImmediate(bodyParts.rightHand.GetChild(0).gameObject);
    }
    /*
    void StandardizeNaming_IfEntireHierachyMatches(Transform rig, Transform correctlyNamedRig)
    {
        Transform[] bones = rig.GetComponentsInChildren<Transform>();
        Transform[] correctlyNamedBones = correctlyNamedRig.GetComponentsInChildren<Transform>();

        if (bones.Length != correctlyNamedBones.Length)
        {
            Debug.LogWarning($"Invalid transform ranaming, they need to match in their transform hierarchies." +
                             $"source = {rig.name} has {bones.Length} children, but destination = " +
                             $"{correctlyNamedRig} has {correctlyNamedBones.Length} children.");

            string[] rigNames = bones.Select(s => s.name.Replace("(Clone)", "")).ToArray();
            string[] correctNames = correctlyNamedBones.Select(d => d.name.Replace("(Clone)", "")).ToArray();
            rigNames.Where(s => !correctNames.Contains(s)).ToArray().Print("Only in source: ");
            correctNames.Where(d => !rigNames.Contains(d)).ToArray().Print("Only in destination: ");
            return;
        }

        for (int i = 0; i < bones.Length; i++)
            bones[i].name = correctlyNamedBones[i].name;
    }


    void StandardizeNaming_WhenEverPossible(Transform rig, Transform correctlyNamedRig)
    {
        Transform[] bones = rig.GetComponentsInChildren<Transform>();
        Transform[] correctlyNamedBones = correctlyNamedRig.GetComponentsInChildren<Transform>();
        List<Transform> failedBones = new List<Transform>();

        int count = Mathf.Min(bones.Length, correctlyNamedBones.Length);

        for (int i = 0; i < count; i++)
        {
            if (bones[i].name.Contains(correctlyNamedBones[i].name))
                bones[i].name = correctlyNamedBones[i].name;
            else
                failedBones.Add(bones[i]);
        }

        if (bones.Length > count)
            for (int i = count; i < bones.Length; i++)
                failedBones.Add(bones[i]);
        if (correctlyNamedBones.Length > count)
            for (int i=count; i<correctlyNamedBones.Length; i++)
                failedBones.Add(correctlyNamedBones[i]);

        if (failedBones.Count > 0)
        {
            Debug.LogWarning($"Failed to rename the following {failedBones.Count} bones");
            failedBones.Print();
        }

    }
    */
}
#endif