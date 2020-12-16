using System.Collections.Generic;
using UnityEngine;

//The idea is to find, whenever a 1 is found, all the elements of that island through a recursive function and keep track of them into m_IsSlotsAlreadyBeenChecked.
//Thus, when the array check is the resumed, the already finded 1 are not taken into account
public class IslandTest : MonoBehaviour
{
    private const string FILE_PATH = "Assets/Resources/IslandProblem.txt";
    private const int N = 20;
    private int[,] m_data = new int[N, N];
    private bool[,] m_IsSlotAlreadyBeenChecked = new bool[N, N];
    private int m_IslandCount = 0;

    private void Awake()
    {
        ReadFromFile();
        FindIslandCount();
    }

    private void ReadFromFile()
    {
        List<string> fileLines = new List<string>(System.IO.File.ReadAllLines(FILE_PATH));

        for (int raw = 0; raw < N; ++raw)
        {
            string line = fileLines[raw];
            for (int col = 0; col < N; ++col)
            {
                int i = (int)line[col];
                m_data[raw, col] = (i == 48) ? 0 : 1;    //This is not very elegant, it is working though and allowed me to procede with the task
                m_IsSlotAlreadyBeenChecked[raw, col] = false;
            }
        }
    }

    private void FindIslandCount()
    {
        for (int raw = 0; raw < N; ++raw)
        {
            for (int col = 0; col < N; ++col)
            {
                if (m_data[raw, col] == 1) //if the slot is one
                {
                    if (!m_IsSlotAlreadyBeenChecked[raw, col]) //if the slot is not already been checked => this means that is the first element of an island
                    {
                        CheckCloseSlots(raw, col); //Recursive function to find every 1 belonging to the island
                        ++m_IslandCount;
                    }
                }
            }
        }
        PrintIslandCount();
    }

    private void CheckCloseSlots(int raw, int col)
    {
        m_IsSlotAlreadyBeenChecked[raw, col] = true; //Set the slot as checked

        if (raw != 0 && m_data[raw - 1, col] == 1 && !m_IsSlotAlreadyBeenChecked[raw - 1, col])
            CheckCloseSlots(raw - 1, col); // Check the slot on the top
        if (raw != N - 1 && m_data[raw + 1, col] == 1 && !m_IsSlotAlreadyBeenChecked[raw + 1, col])
            CheckCloseSlots(raw + 1, col);  // Check the slot on the bottom
        if (col != 0 && m_data[raw, col - 1] == 1 && !m_IsSlotAlreadyBeenChecked[raw, col - 1])
            CheckCloseSlots(raw, col - 1);  // Check the slot on the left
        if (col != N - 1 && m_data[raw, col + 1] == 1 && !m_IsSlotAlreadyBeenChecked[raw, col + 1])
            CheckCloseSlots(raw, col + 1);  // Check the slot on the right
    }

    private void PrintIslandCount()
    {
        Debug.Log($"There are {m_IslandCount} islands");
    }
}