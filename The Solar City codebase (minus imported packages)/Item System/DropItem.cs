using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] bool dontDropIfPossessed;
    [SerializeField] float dropChance = 1;
    [SerializeField] Item[] items = new Item[1];
    [SerializeField] float[] probabilitiesGivenDrop = new float[1];

    ADamagable damagable;
    ProbabilityDistribution probabilitiesGivenDrop_;

    void Start()
    {
        if (items.Length != probabilitiesGivenDrop.Length)
            Debug.LogError(
                $"items.Length should equal probabilitiesGivenDrop.Length, " +
                $"which is violated by {transform.Path()}");

        damagable = this.GetComponentInCharacter<ADamagable>();
        probabilitiesGivenDrop_ = new ProbabilityDistribution(probabilitiesGivenDrop);
        damagable.OnDie += OnDie;
    }

    private void OnDestroy()
    {
        damagable.OnDie -= OnDie;
    }


    void OnDie()
    {
        if (items.Length == 0)
            return;
        if (Random.value > dropChance)
            return;
        if (dropChance == 0)
            return;

        Item item = items[probabilitiesGivenDrop_.Draw()];
        Drop(item);
    }


    void Drop(Item item)
    {
        bool isPossessed = Inventory.slots[item].itemCount > 0;
        if (isPossessed && dontDropIfPossessed)
            return;

        var itemObject = Instantiate(ItemPrefabs.instance.temporaryItemPrefab);
        float randomDistance = 0.9f * GetComponent<CapsuleCollider>()?.radius ?? 0.1f;
        float randomAngle = Random.Range(0, 2 * Mathf.PI);
        Vector3 randomOffset = randomDistance * new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
        itemObject.transform.position = transform.position + randomOffset;
        itemObject.name = item.name;
        itemObject.GetComponent<ItemPickup>().item = item;
    }



}
