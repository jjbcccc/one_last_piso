using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float framesPerSecond = 12f;
    [SerializeField] private bool loop = true;

    private SpriteRenderer sr;
    private float timer;
    private int frameIndex;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;

            if (frameIndex >= frames.Length)
            {
                if (loop) frameIndex = 0;
                else frameIndex = frames.Length - 1;
            }
        }

        sr.sprite = frames[frameIndex];
    }

    public void SetFrames(Sprite[] newFrames)
    {
        frames = newFrames;
        frameIndex = 0;
        timer = 0f;
    }
}