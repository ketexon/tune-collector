using UnityEngine;
using UnityEngine.UI;

public class Tune : MonoBehaviour
{
	[SerializeField] Transform damageCountEntryContainer;
	[SerializeField] BlockSpawner blockSpawner;
	[SerializeField] TMPro.TMP_Text tuneNameText;
	[SerializeField] GameObject damageCountEntryPrefab;
	[SerializeField] Color melodyColor = Color.cyan;
	[SerializeField] Color bassColor = Color.yellow;
	[SerializeField] Color percussionColor = Color.red;

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
			var text = damageCountEntry.GetComponentInChildren<TMPro.TMP_Text>();
			var image = damageCountEntry.GetComponentInChildren<Image>();
			text.text = $"{dmg.Damage}";
			image.color = dmg.Type switch
			{
				TuneType.Melody => melodyColor,
				TuneType.Bass => bassColor,
				TuneType.Percussion => percussionColor,
				_ => Color.white
			};
		}
	}
}
