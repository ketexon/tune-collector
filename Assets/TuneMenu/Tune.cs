using UnityEngine;
using UnityEngine.UI;

public class Tune : MonoBehaviour
{
	[SerializeField] Transform damageCountEntryContainer;
	[SerializeField] BlockSpawner blockSpawner;
	[SerializeField] TMPro.TMP_Text tuneNameText;
	[SerializeField] DamageCountEntry damageCountEntryPrefab;
	[SerializeField] Theme theme;

	void Start()
	{
		foreach (Transform child in damageCountEntryContainer)
		{
			Destroy(child.gameObject);
		}

		var measure = blockSpawner.blockPrefab.GetComponent<Measure>();
		tuneNameText.text = measure.TuneName;

		foreach (var dmg in measure.Damage)
		{
			var damageCountEntry = Instantiate(damageCountEntryPrefab, damageCountEntryContainer);
			damageCountEntry.Damage = dmg.Damage;
			damageCountEntry.Type = dmg.Type;
		}
	}
}
