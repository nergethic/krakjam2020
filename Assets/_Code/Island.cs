using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField] Direction direction;
    [SerializeField] float floatingSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        var pos = transform.position;

        switch (direction) {
            case Direction.Left:
                pos.x += Time.deltaTime * floatingSpeed;
                break;
            
            case Direction.Right:
                break;
        }

        transform.position = pos;
    }

    enum Direction {
        Left,
        Right
    }
}
