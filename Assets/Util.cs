using UnityEngine;

public static class Util
{
	public static void Delay(this MonoBehaviour mb, float seconds, System.Action callback)
	{
		mb.StartCoroutine(Impl());
		System.Collections.IEnumerator Impl()
		{
			yield return new WaitForSeconds(seconds);
			callback();
		}
	}

	public static void Shuffle(this System.Collections.IList list) {
		int n = list.Count;
		while (n > 1) {
			int k = Random.Range(0, n--);
			(list[k], list[n]) = (list[n], list[k]);
		}
	}
}