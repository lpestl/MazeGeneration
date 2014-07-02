﻿using UnityEngine;
using System.Collections;

public class ScreenController : MonoBehaviour {
    public GameObject joystickMove;
    public Transform followTransform;

    public Transform floorParent;
    public Transform wallParent;

    public string namePrefabWall;
    public string namePrefabFloor;

    private int _wLab = 10;
    private int _hLab = 10;

	// Use this for initialization
	void Start () {
        var radJ = joystickMove.GetComponent<EasyJoystick>().zoneRadius;
        joystickMove.GetComponent<EasyJoystick>().joystickPosition = new Vector2(Screen.width - radJ - 20, Screen.height / 2);

        loadFloor();

        loadWalls();
	}
	
	// Update is called once per frame
	void Update () {
        var radJ = joystickMove.GetComponent<EasyJoystick>().zoneRadius;
        joystickMove.GetComponent<EasyJoystick>().joystickPosition = new Vector2(Screen.width - radJ - 20, Screen.height / 2);

        transform.position = new Vector3(followTransform.position.x, transform.position.y, followTransform.position.z);
	}

    void loadFloor()
    {
        for (int i = 0; i < _wLab; i++)
        {
            for (int j = 0; j < _hLab; j++)
            {
                GameObject fl = Instantiate(Resources.Load(namePrefabFloor)) as GameObject;

                fl.transform.position = new Vector3(i, 0, j);
                fl.transform.parent = floorParent;

                Resources.UnloadUnusedAssets();
            }
        }        
    }

    void loadWall(int _i, int _j, int _ang) {
        GameObject wl = Instantiate(Resources.Load("wall")) as GameObject;

        wl.transform.position = new Vector3(_i, 0, _j);
        wl.transform.parent = wallParent;
        wl.transform.localEulerAngles = new Vector3(0, _ang, 0);

        Resources.UnloadUnusedAssets();
    }

    void loadWalls()
    {
        // Границы по периметру
        for (int i = 0; i < _wLab; i++) {
            for (int j = 0; j < _hLab; j++) {
                if (i == 0) {
                    loadWall(i, j, 90);
                }

                if ((i == _wLab - 1) && (j != 0)) {
                    loadWall(i, j, 270);
                }

                if (j == 0) {
                    loadWall(i, j, 0);
                }

                if (j == _hLab - 1) {
                    loadWall(i, j, 180);
                }
            }
        }
        
        int[] line = new int[_wLab];
        int[] cell_floor = new int[_wLab];
        int[] cell_wall = new int[_wLab];
        int many = 1;

        //Создайте первую строку. Ни одна ячейка не будет являться частью ни одного множества.
        //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
        for (int j = 0; j < _wLab; j++) {
            line[j] = many;
            many++;
            cell_wall[j] = 0;
            cell_floor[j] = 0;
        }


        for (int i = 0; i < _hLab - 1; i++) {
            //Присвойте ячейкам, не входящим в множество, свое уникальное множество.
            for (int j = 0; j < _wLab; j++) {
                if (line[j] == 0) {
                    many++;
                    line[j] = many;
                }
            }

            //Создайте правые границы, двигаясь слева направо:
            //Случайно решите добавлять границу или нет
            for (int j = 0; j < _wLab; j++) {

                cell_wall[j] = Random.Range(0, 2);
                cell_floor[j] = Random.Range(0, 2);
            }

            for (int j = 0; j < _wLab - 1; j++) {

                //Если текущая ячейка и ячейка справа принадлежат одному множеству, то создайте границу между ними (для предотвращения зацикливаний)
                if (line[j] == line[j + 1]) {
                    cell_wall[j] = 1;
                }

                //Если вы решили не добавлять границу, то объедините два множества в которых находится текущая ячейка и ячейка справа.
                if (cell_wall[j] == 0) {
                    var chln = line[j + 1];
                    for (int k = 0; k < _wLab; k++) {
                        if (line[k] == chln) {
                            line[k] = line[j];
                        }
                    }
                }
            }

            //Создайте границы снизу, двигаясь слева направо:
            //Случайно решите добавлять границу или нет. Убедитесь что каждое множество имеет хотя бы одну ячейку без нижней границы (для предотвращения изолирования областей)
            int[] count_hole = new int[_wLab];
            int[] count_line = new int[_wLab];

            for (int j = 0; j < _wLab; j++) {
                count_hole[j] = 0;
                count_line[j] = 0;

                for (int k = 0; k < _wLab; k++) {
                    if (line[k] == line[j]) {
                        count_line[j]++;
                        if (cell_floor[k] == 0) {
                            count_hole[j]++;
                        }
                    }
                }

                //Убедитесь что каждое множество имеет хотя бы одну ячейку без нижней границы (для предотвращения изолирования областей)
                if (count_hole[j] == 0) {
                    cell_floor[j] = 0;
                }

                //Если ячейка в своем множестве одна, то не создавайте границу снизу
                if (count_line[j] == 1) {
                    cell_floor[j] = 0;
                }

                //Если ячейка одна в своем множестве без нижней границы, то не создавайте нижнюю границу
            }
            
            //Выведите текущую строку
            for (int j = 0; j < _wLab; j++) {
                if ((cell_wall[j] == 1) && (j != _wLab-1)) {
                    loadWall(j, i, 270);
                }
                if (cell_floor[j] == 1) {
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
        for (int j = 0; j < _wLab; j++)
        {
            if (line[j] == 0)
            {
                many++;
                line[j] = many;
            }
            //Debug.Log(line[j]);
        }

        //Если вы решите закончить лабиринт, то:
        //Добавьте нижнюю границу к каждой ячейке
        //Двигаясь слева направо:
        //Если текущая ячейка и ячейка справа члены разных множеств, то:
        //Удалите правую границу
        //Объедините множества текущей ячейки и ячейки справа
        //Выведите завершающую строку
        for (int i = 0; i < _wLab - 1; i++)
        {
            cell_wall[i] = 1;

            if (line[i] != line[i + 1])
            {
                cell_wall[i] = 0;

                var chln = line[i + 1];
                for (int k = 0; k < _wLab; k++)
                {
                    if (line[k] == chln)
                    {
                        line[k] = line[i];
                    }
                }
            }

            if (cell_wall[i] == 1)
            {
                loadWall(i, _hLab - 1, 270);
            }
        }

        //for (int cr_l = 0; cr_l < _hLab; cr_l++)
        //{
        //    for (int i = 0; i < _wLab; i++)
        //    {
        //        if (cr_l == 0) line[i] = 0;

        //        //cell.clearRect(13 * i + 3, 13 * cr_l + 3, 10, 10);

        //        cell_wall[i] = 0;
        //        if (cell_floor[i] == 1)
        //        {
        //            cell_floor[i] = 0;
        //            line[i] = 0;
        //        }

        //        if (line[i] == 0) line[i] = many++;
        //    }

        //    for (int i = 0; i < _wLab; i++)
        //    {
        //        cell_wall[i] = Random.Range(0, 2); //Math.floor(Math.random() * 2);
        //        cell_floor[i] = Random.Range(0, 2);

        //        if (((cell_wall[i] == 0) || (cr_l == _hLab - 1)) && (i != _wLab - 1) && (line[i + 1] != line[i]))
        //        {
        //            var temp_line = line[i + 1];

        //            for (int j = 0; j < _wLab; j++)
        //                if (line[j] == temp_line) line[j] = line[i];

        //            //cell.clearRect(13 * i + 3, 13 * cr_l + 3, 15, 10);
        //        }
        //        else
        //        {
        //            GameObject wl = Instantiate(Resources.Load("wall")) as GameObject;

        //            wl.transform.position = new Vector3(cr_l, 0, i);
        //            wl.transform.parent = wallParent;
        //            wl.transform.localEulerAngles = new Vector3(0, 270, 0);

        //            Resources.UnloadUnusedAssets();
        //        }

        //        if ((cr_l != _hLab - 1) && (cell_floor[i] == 0))
        //        {
        //            //cell.clearRect(13 * i + 3, 13 * cr_l + 3, 10, 15);
        //        }
        //        else
        //        {
        //            GameObject wl = Instantiate(Resources.Load("wall")) as GameObject;

        //            wl.transform.position = new Vector3(cr_l, 0, i);
        //            wl.transform.parent = wallParent;
        //            wl.transform.localEulerAngles = new Vector3(0, 180, 0);

        //            Resources.UnloadUnusedAssets();
        //        }
        //    }

        //    for (int i = 0; i < _wLab; i++)
        //    {
        //        var count_floor = 0;
        //        var count_hole = 0;

        //        for (int j = 0; j < _wLab; j++)
        //            if ((line[i] == line[j]) && (cell_floor[j] == 0)) count_hole++;
        //            else count_floor++;

        //        if (count_hole == 0)
        //        {
        //            cell_floor[i] = 0;
        //            //cell.clearRect(13 * i + 3, 13 * cr_l + 3, 10, 15);
        //        }
        //        else
        //        {
        //            GameObject wl = Instantiate(Resources.Load("wall")) as GameObject;

        //            wl.transform.position = new Vector3(cr_l, 0, i);
        //            wl.transform.parent = wallParent;
        //            wl.transform.localEulerAngles = new Vector3(0, 180, 0);

        //            Resources.UnloadUnusedAssets();
        //        }

        //    }
        //}

    }

}
