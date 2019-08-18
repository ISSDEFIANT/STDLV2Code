using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildAnimationScript : MonoBehaviour
{
    MeshFilter mf;

    public float BuildTime;         // Время анимации в секундах


	public bool Activate;

    private bool Active = false;    // Работает ли анимация
    private float beginTime;        // В какой момент времени была начата анимация
    private List<int> oldTriangles; // Кэш для первоначальных полигонов
    
    //Активирует анимацию
    public void SetActive()
    {
        beginTime = Time.time;      // Запоминаем начало анимации
        Active = true;              // Активируем анимацию
    }

    void Start()
    {
        mf = gameObject.GetComponent<MeshFilter>(); //кэшируем ссылку на MeshFilter
        oldTriangles = mf.mesh.triangles.ToList();  //Запоминаем первоначальные треугольники
        
        mf.mesh.triangles = new int[0];             //Говорим мешу что у него больше нет треугольников - и вуаля, модель не видно
    }

    void Update()
    {
		if (Activate) {
			SetActive ();
			Activate = false;
		}


        if (!Active)    //Если анимация ещё не активирована, то ничего не делаем
            return;
        
        float percentage = Mathf.Clamp01((Time.time - beginTime) / BuildTime);  //получаем процент строительства к данному времени (в районе 0-1)

        int trianglesToBuild = (int)(percentage * (oldTriangles.Count / 3));    //количество треугольников(не вершин) для строительства

        mf.mesh.triangles = oldTriangles.Take(trianglesToBuild * 3).ToArray();  //применяем новые треугольники

        if (percentage == 1)    //если строительство завершилось, можно закончить работу скрипта
        {
            Active = false;
            //Destroy(this); //Если корабль собрался, и нам не нужен конкретно скрипт, то можно удалить скрипт у корабля.
        }
    }
}
