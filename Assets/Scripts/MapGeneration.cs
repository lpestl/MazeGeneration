using UnityEngine;
using System.Collections;

public class MapGeneration : MonoBehaviour {
    public Transform floorParent;
    public Transform wallParent;
    public Transform endTrigger;

    public string namePrefabWall;
    public string namePrefabFloor;
    public string nameStolbWall;

    public int widthLab = 10;
    public int heightLab = 10;

	// Use this for initialization
	void Start () {
        var lvl = GameObject.Find("FPSandCONTROL").GetComponent<SceneController>();
        widthLab = lvl.wMaze;
        heightLab = lvl.hMaze;

        loadFloor();
        loadWalls();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void loadFloor()
    {
        for (int i = 0; i < widthLab; i++)
        {
            for (int j = 0; j < heightLab; j++)
            {
                GameObject fl = Instantiate(Resources.Load(namePrefabFloor)) as GameObject;

                fl.transform.position = new Vector3(i, 0, j);
                fl.transform.parent = floorParent;

                Resources.UnloadUnusedAssets();
            }
        }
    }

    void loadWall(int _i, int _j, int _ang)
    {
        GameObject wl = Instantiate(Resources.Load(namePrefabWall)) as GameObject;

        wl.transform.position = new Vector3(_i, 0, _j);
        wl.transform.parent = wallParent;
        wl.transform.localEulerAngles = new Vector3(0, _ang, 0);

        Resources.UnloadUnusedAssets();
    }

    void loadStolb(int _i, int _j)
    {
        GameObject wl = Instantiate(Resources.Load(nameStolbWall)) as GameObject;

        wl.transform.position = new Vector3(_i, 0, _j);
        wl.transform.parent = wallParent;
        wl.transform.localEulerAngles = new Vector3(0, 0, 0);

        Resources.UnloadUnusedAssets();
    }

    void loadWalls()
    {
        // Границы по периметру
        for (int i = 0; i < widthLab; i++)
        {
            for (int j = 0; j < heightLab; j++)
            {
                if (i == 0)
                {
                    loadWall(i, j, 90);
                }

                if ((i == widthLab - 1) && (j != 0))
                {
                    loadWall(i, j, 270);
                }

                if (j == 0)
                {
                    loadWall(i, j, 0);
                }

                if (j == heightLab - 1)
                {
                    loadWall(i, j, 180);
                }
            }
        }

        int[] line = new int[widthLab];
        int[] cell_floor = new int[widthLab];
        int[] cell_wall = new int[widthLab];
        int many = 1;

        //Создайте первую строку. Ни одна ячейка не будет являться частью ни одного множества.
        //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
        for (int j = 0; j < widthLab; j++)
        {
            line[j] = many;
            many++;
            cell_wall[j] = 0;
            cell_floor[j] = 0;
        }


        for (int i = 0; i < heightLab - 1; i++)
        {
            //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
            for (int j = 0; j < widthLab; j++)
            {
                if (line[j] == 0)
                {
                    many++;
                    line[j] = many;
                }
            }

            //Создайте правые границы, двигаясь слева направо:
            //Случайно решите добавлять границу или нет
            for (int j = 0; j < widthLab; j++)
            {

                cell_wall[j] = Random.Range(0, 2);
                cell_floor[j] = Random.Range(0, 2);
            }

            for (int j = 0; j < widthLab - 1; j++)
            {

                //Если текущая ячейка и ячейка справа принадлежат одному множеству, то создайте границу между ними (для предотвращения зацикливаний)
                if (line[j] == line[j + 1])
                {
                    cell_wall[j] = 1;
                }

                //Если вы решили не добавлять границу, то объедините два множества в которых находится текущая ячейка и ячейка справа.
                if (cell_wall[j] == 0)
                {
                    var chln = line[j + 1];
                    for (int k = 0; k < widthLab; k++)
                    {
                        if (line[k] == chln)
                        {
                            line[k] = line[j];
                        }
                    }
                }
            }

            //Создайте границы снизу, двигаясь слева направо:
            //Случайно решите добавлять границу или нет. Убедитесь что каждое множество имеет хотя бы одну ячейку без нижней границы (для предотвращения изолирования областей)
            int[] count_hole = new int[widthLab];
            int[] count_line = new int[widthLab];

            for (int j = 0; j < widthLab; j++)
            {
                count_hole[j] = 0;
                count_line[j] = 0;

                for (int k = 0; k < widthLab; k++)
                {
                    if (line[k] == line[j])
                    {
                        count_line[j]++;
                        if (cell_floor[k] == 0)
                        {
                            count_hole[j]++;
                        }
                    }
                }

                //Убедитесь что каждое множество имеет хотя бы одну ячейку без нижней границы (для предотвращения изолирования областей)
                if (count_hole[j] == 0)
                {
                    cell_floor[j] = 0;
                }

                //Если ячейка в своем множестве одна, то не создавайте границу снизу
                if (count_line[j] == 1)
                {
                    cell_floor[j] = 0;
                }

                //Если ячейка одна в своем множестве без нижней границы, то не создавайте нижнюю границу
            }

            //Выведите текущую строку
            for (int j = 0; j < widthLab; j++)
            {
                if ((cell_wall[j] == 1) && (j != widthLab - 1))
                {
                    loadWall(j, i, 270);
                }
                if (cell_floor[j] == 1)
                {
                    loadWall(j, i, 180);
                }

                //Удалите все правые границы
                //Удалите ячейки с нижней границей из их множества
                //Удалите все нижние границы
                cell_wall[j] = 0;
                if (cell_floor[j] == 1) line[j] = 0;
                cell_floor[j] = 0;
                //Продолжайте с шага 2
            }
        }

        //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
        for (int j = 0; j < widthLab; j++)
        {
            if (line[j] == 0)
            {
                many++;
                line[j] = many;
            }
        }

        //Если вы решите закончить лабиринт, то:
        //Добавьте нижнюю границу к каждой ячейке
        //Двигаясь слева направо:
        //Если текущая ячейка и ячейка справа члены разных множеств, то:
        //Удалите правую границу
        //Объедините множества текущей ячейки и ячейки справа
        //Выведите завершающую строку
        for (int i = 0; i < widthLab - 1; i++)
        {
            cell_wall[i] = 1;

            if (line[i] != line[i + 1])
            {
                cell_wall[i] = 0;

                var chln = line[i + 1];
                for (int k = 0; k < widthLab; k++)
                {
                    if (line[k] == chln)
                    {
                        line[k] = line[i];
                    }
                }
            }

            if (cell_wall[i] == 1)
            {
                loadWall(i, heightLab - 1, 270);
            }
        }

        for (int i = 0; i < widthLab - 1; i++)
        {
            for (int j = 0; j < heightLab - 1; j++)
            {
                loadStolb(i, j);
            }
        }

        endTrigger.position = new Vector3(endTrigger.position.x + widthLab - 1, endTrigger.position.y, endTrigger.position.z);
    }
}
