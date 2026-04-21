using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private bool loop = true;

    private SpriteRenderer _spriteRenderer;
    private int _currentFrame;
    private float _timer;
    private bool _isPlaying = true;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!_isPlaying || sprites.Count == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= frameRate)
        {
            _timer -= frameRate;
            _currentFrame++;

            if (_currentFrame >= sprites.Count)
            {
                if (loop)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _isPlaying = false;
                    _currentFrame = sprites.Count - 1;
                }
            }

            _spriteRenderer.sprite = sprites[_currentFrame];
        }
    }

    public void Play(List<Sprite> newSprites, bool shouldLoop = true)
    {
        sprites = newSprites;
        loop = shouldLoop;
        _currentFrame = 0;
        _timer = 0;
        _isPlaying = true;
    }

    public void Stop()
    {
        _isPlaying = false;
    }
}
