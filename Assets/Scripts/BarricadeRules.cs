using UnityEngine;

public class BarricadeRules : MonoBehaviour
{
    public int hp = 2;
    public SpriteRenderer sr;
    public Sprite dmg_sprite;
    public string barricade_side;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.gameObject);
        hp--;
        if (hp == 1)
        {
            // damaged is left
            //fine is right
            //if obj is right, flip damaged
            //if obj is left, reFlip to get left side
            if (barricade_side.Equals("left"))
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX=true;
            }
            sr.sprite = dmg_sprite;
            //Debug.Log("still some fight left");
        }
        else
        {
            //Debug.Log("Bye shield...");
            Destroy(gameObject);
        }

        /*
         * when shot:
         *  -shake?
         *  if hp= 1 change skin
         *  if hp=0, kill
        */
    }



}
