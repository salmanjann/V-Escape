using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropglow : MonoBehaviour
{
    private bool dropping;
    public GameObject glowstick_prefab;
    private List<GameObject> glowsticks;
    private int index;
    private int max;
    // Start is called before the first frame update
    void Start()
    {
        dropping = false;
        index = 0;
        max = 1000;
        glowsticks = new List<GameObject>();
        for (int i = 0; i < max; i++)
        {
            glowsticks.Add(Instantiate(glowstick_prefab));
            glowsticks[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dropping && Input.GetKey(KeyCode.C))
        {
            dropping = true;
            glowsticks[index % max].SetActive(true);
            glowsticks[index % max].transform.rotation = this.transform.rotation;
            glowsticks[index % max].transform.Rotate(90, 0, 0);
            glowsticks[index++ % max].transform.position = this.transform.position + Vector3.one/3;
        }
        if (dropping && !Input.GetKey(KeyCode.C))
        {
            dropping = false;
        }
    }
}
