using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
	public GameObject ghostPrefab;
	public float ghostLifetime = 0.4f;

	public void SpawnGhost(Vector3 position, SpriteRenderer sourceRenderer, Vector3 localScale)
	{
		GameObject ghost = Instantiate(ghostPrefab, position, Quaternion.identity);
		SpriteRenderer ghostRenderer = ghost.GetComponent<SpriteRenderer>();

		if (ghostRenderer != null && sourceRenderer != null)
		{
			ghostRenderer.sprite = sourceRenderer.sprite;
			ghostRenderer.flipX = sourceRenderer.flipX;
			ghostRenderer.color = new Color(1f, 1f, 1f, 0.5f);
		}

		ghost.transform.localScale = localScale;
		Destroy(ghost, ghostLifetime);
	}
}