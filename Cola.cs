using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListasTipoCola1
{
    class Cola
    {
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

        /*
        public void Imprimir()
        {
            Nodo reco = raiz;
            Console.WriteLine("Listado de todos los elementos de la cola.");
            while (reco != null)
            {
                Console.Write(reco.info + "-");
                reco = reco.sig;
            }
            Console.WriteLine();
        }

        
        static void Main(string[] args)
        {
            Cola cola1 = new Cola();
            cola1.Insertar(5);
            cola1.Insertar(10);
            cola1.Insertar(50);
            cola1.Imprimir();
            Console.WriteLine("Extraemos uno de la cola:" + cola1.Extraer());
            cola1.Imprimir();
            Console.ReadKey();
        }
        */
    }
}