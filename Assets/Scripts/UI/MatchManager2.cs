using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager2 : MonoBehaviour
{
    private int level;
    private int numberOfPlayers;
    private List<int> characters;
    private float punctuationTeam1;
    private float punctuationTeam2;
    private float initialTime;
    public List<int> team1Dishes;
    public List<int> team2Dishes;

    MenuBehaviour menuBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        characters = new List<int>();
        team1Dishes = new List<int>();
        team2Dishes = new List<int>();

        initialTime = 300.0f;

        menuBehaviour = GetComponent<MenuBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int generateOrder()
    {
        Random.seed = (int)(100 + Time.deltaTime);
        int difficultyLevel = 1;
        int randomProduct = 0;
        int randomLevel = 0;
        for (int i = 0; i < this.level; i++)
        {
            randomProduct = (int)Random.Range(0.0f, ProductManager.finalProducts.Count);
            randomLevel = (int)Random.Range(0.0f, ProductManager.finalProducts.Count);
            if (difficultyLevel < randomLevel)
            {
                difficultyLevel = randomLevel;
            }
        }
        return randomProduct;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }


    public int getLevel()
    {
        return this.level;

    }


    public void setNumberOfPlayers(int numberOfPlayers)
    {
        this.numberOfPlayers = numberOfPlayers;
    }


    public int getNumberOfPlayers()
    {
        return this.numberOfPlayers;

    }

    public void setCharacters(List<int> characters)
    {
        this.characters = characters;
    }


    public List<int> getCharacters()
    {
        return this.characters;

    }

    public float getPunctuationTeam1()
    {
        return this.punctuationTeam1;
    }

    public void setPunctuationTeam1(float punctuationTeam1)
    {
        this.punctuationTeam1 = punctuationTeam1;
    }

    public float getPunctuationTeam2()
    {
        return this.punctuationTeam2;
    }

    public void setPunctuationTeam2(float punctuationTeam2)
    {
        this.punctuationTeam2 = punctuationTeam2;
    }

    public float getInitialTime()
    {
        return this.initialTime;
    }

    public void setInitialTime(float initialTime)
    {
        this.initialTime = initialTime;
    }

    public bool deliverProduct(int team, int id)
    {
        if (team == 1 && team1Dishes.Contains(id))
        {
            team1Dishes.Remove(id);
            punctuationTeam1 += (ProductManager.finalProducts[id].difficulty+1) * 10;
            menuBehaviour.updatePoints();
            return true;
        }
        else if (team == 2 && team2Dishes.Contains(id))
        {
            team2Dishes.Remove(id);
            punctuationTeam2 += (ProductManager.finalProducts[id].difficulty + 1) * 10;
            menuBehaviour.updatePoints();
            return true;
        }

        return false;
    }

}
