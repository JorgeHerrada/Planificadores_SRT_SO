using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListasTipoCola1
{
    class Cola
    {
        public int size = 0;
        List<int> listaTerminados = new List<int>();
        class Nodo
        {
            public string id;
            public string nombre;
            public string duracion;
            public Nodo sig;
        }

        private Nodo raiz, fondo;

        public Cola()
        {
            raiz = null;
            fondo = null;
        }

        public bool Vacia()
        {
            if (raiz == null)
                return true;
            else
                return false;
        }

        public void Insertar(string id, string nombre, string duracion)
        {
            size++;
            Nodo nuevo;
            nuevo = new Nodo();
            nuevo.id = id;
            nuevo.nombre = nombre;
            nuevo.duracion = duracion;
            nuevo.sig = null;
            if (Vacia())
            {
                raiz = nuevo;
                fondo = nuevo;
            }
            else
            {
                fondo.sig = nuevo;
                fondo = nuevo;
            }
        }

        public string[] Extraer()
        {
            if (!Vacia())
            {
                size--;
                string[] informacion = { raiz.id, raiz.nombre, raiz.duracion };
                if (raiz == fondo)
                {
                    raiz = null;
                    fondo = null;
                }
                else
                {
                    raiz = raiz.sig;
                }
                return informacion;
            }
            else
            {
                string[] informacion = { "0", "0", "0" };
                Console.WriteLine("La cola está vacía");
                return informacion;
            }
        }

        // ordenar la cola en orden ascendiente de duración
        public int IndiceSRT()
        {
            // si la cola esta vacia no actuamos, retorna -1
            if (Vacia())
            {
                Console.WriteLine("La cola está vacía");
                return -1;
            }

            // creamos nodo auxiliar para recorrer cola
            Nodo nuevo;
            nuevo = raiz;

            // recorre la cola  y retorna el indice del trabajo con menor tiempo restante
            int srt = 999999;
            int indiceMenor = -1;
            for(int i = 0; i < size; i++)
            {
                if (!listaTerminados.Contains(i))
                {
                    if (Int32.Parse(nuevo.duracion) < srt)
                    {
                        srt = Int32.Parse(nuevo.duracion);
                        indiceMenor = i;
                    }
                }
                nuevo = nuevo.sig;
            }
            Console.WriteLine(indiceMenor);
            return indiceMenor;
        }

        // añadimos indice a lista de procesos terminados
        public void ProcesoTerminado(int indice)
        {
            listaTerminados.Add(indice);
        }

    }

}