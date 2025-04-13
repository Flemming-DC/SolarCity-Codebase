using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Singleton/ItemPrefabs")]
public class ItemPrefabs : ScriptableObjectSingleton<ItemPrefabs>
{
    public GameObject temporaryItemPrefab;
    //public GameObject permanentItemPrefab;


}
