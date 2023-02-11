using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NoteController : MonoBehaviour
{
	public NotesManager notesManager;
	public bool menu;

	public Vector2 playPoint;
	public Song.Note note;
	public float speed;
    public PianoKey key;
	bool playing = false;

	public SpriteRenderer renderer;
	public Light2D light;
	public BoxCollider2D collider;

	public bool played = false;
	public Color unplayedColor;
	public Color playedColor;

	bool tryBindToKey = false;

	public void Start()
	{
		menu = FindObjectOfType<GameManager>().menu;

		if (!menu)
		{
			renderer.color = unplayedColor;
			light.color = unplayedColor;
		}
	}

	void Update()
    {
		// tries to bind to key
		if (tryBindToKey && !key.noteController)
		{
			// binds key to note
			key.pitch = note.pitch;
			key.noteController = this;
			tryBindToKey = false;
		}

        // moves down
        transform.position = new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime);

        // checks if bottom reached play point
        if(!playing && (transform.position.y + (renderer.sprite.bounds.min.y * transform.lossyScale.y)) <= playPoint.y)
        {
			if (menu)
            {
				key.pitch = note.pitch;
				playing = true;
				// plays piano key
				key.StartSound();
			}
        }

		// checks if top reached play
		if ((transform.position.y + renderer.sprite.bounds.max.y * transform.lossyScale.y) <= playPoint.y)
		{
            if (menu)
            {
				// stops piano
				key.Stop();
				Destroy(gameObject);
			}
			else
			{
				if (played)
				{
					Destroy(gameObject);
				}
				else
				{
					// level failed
					FindObjectOfType<GameManager>().LevelFailed("You missed a note.");
				}
			}			
		}
	}

	public void OnPlay()
	{
		played = true;

		if (!menu)
		{
			key.noteController = null;
			renderer.color = playedColor;
			light.color = playedColor;
		}
	}

    public void OnNoteTriggerEnter(Collider2D collision)
	{
		if (!menu && collision.CompareTag("PianoKey"))
		{
			tryBindToKey = true;
		}
	}

	public void OnNoteTriggerExit(Collider2D collision)
	{
		if (!menu && collision.CompareTag("PianoKey"))
		{
			key.pitch = 0;
		}
	}

	void OnCollisionStay2D(Collision2D collision)
    {
		key.pitch = note.pitch;

		// checks if key is pressed to its limit
		if (!key.press)
        {
            // disables collider to prevent clipping through
            collider.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if(menu) key.ReleaseKey();
	}
}
