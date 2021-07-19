using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildAnimationScript : MonoBehaviour
{
    public bool Revert;

    MeshFilter mf;

    public float BuildTime; // Время анимации в секундах

    private bool Active = false; // Работает ли анимация
    private float beginTime; // В какой момент времени была начата анимация
    private List<int> oldTriangles; // Кэш для первоначальных полигонов

    public bool usingAmount;
    public float amount;

    //Активирует анимацию
    public void SetActive()
    {
        beginTime = Time.time; // Запоминаем начало анимации
        Active = true; // Активируем анимацию
    }

    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>(); //кэшируем ссылку на MeshFilter
        oldTriangles = mf.mesh.triangles.ToList(); //Запоминаем первоначальные треугольники

        if (!Revert)
        {
            mf.mesh.triangles =
                new int[0]; //Говорим мешу что у него больше нет треугольников - и вуаля, модель не видно
        }

        SetActive();
    }

    void Update()
    {
        if (!Active) //Если анимация ещё не активирована, то ничего не делаем
            return;

        float percentage;
        if (usingAmount)
        {
            percentage = amount;
        }
        else
        {
            percentage = Mathf.Clamp01((Time.time - beginTime) / BuildTime);
        }

        if (Revert) percentage = 1 - percentage;

        int trianglesToBuild =
            (int) (percentage * (oldTriangles.Count / 3)); //количество треугольников(не вершин) для строительства

        mf.mesh.triangles = oldTriangles.Take(trianglesToBuild * 3).ToArray(); //применяем новые треугольники

        if (!usingAmount && !Revert && percentage == 1) //если строительство завершилось, можно закончить работу скрипта
        {
            Active = false;
        }
        else if (!usingAmount && Revert && percentage == 0)
        {
            Active = false;
        }
    }
}
