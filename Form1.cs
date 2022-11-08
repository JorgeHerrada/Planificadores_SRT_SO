using ListasTipoCola1;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Planificadores_SRT_SO
{
    public partial class Form1 : Form
    {
        int contadorProcesos = 0;
        int minutos = 0;
        int segundos = 0;
        int barraActual = 0;
        int duracionGlobal = 0;
        bool iniciado = false;
        //bool moveNext = true;
        bool terminado = false;
        int maximoProcesos = 7;
        Cola cola = new Cola();
        bool[] procesoActivo = { false, false, false, false, false, false, false };

        List<ProgressBar> listaBarras = new List<ProgressBar>();
        List<Label> listaLabels = new List<Label>();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnAgregarProceso_Click(object sender, EventArgs e)
        {
            // Validaciones de entradas
            if (txtNombreNuevoProceso.Text == "" || txtDuracionNuevoProceso.Text == "")
            {
                Console.WriteLine("Asigna nombre y duracion al proceso.");
                return;
            }
            else if (!txtDuracionNuevoProceso.Text.All(char.IsDigit))
            {
                Console.WriteLine(txtDuracionNuevoProceso.Text + " IS NOT A VALID INTEGER.");
                return;
            }

            // Se ha alcanzado el limite de procesos? (7 procesos)
            if (maximoProcesos == contadorProcesos)
            {
                MessageBox.Show("Se ha alcanzado el número máximo de procesos.");
                txtDuracionNuevoProceso.Text = "";
                txtNombreNuevoProceso.Text = "";
                return;
            }

            // Aumenta contador de procesos y asigna a la cola
            contadorProcesos++;
            cola.Insertar(contadorProcesos.ToString(), txtNombreNuevoProceso.Text, txtDuracionNuevoProceso.Text);


            // declaramos proceso activo en registrode banderas
            procesoActivo[contadorProcesos - 1] = true;


            // Agregamos a la list box
            lstNumeroProceso.Items.Add(contadorProcesos);
            lstNombreProceso.Items.Add(txtNombreNuevoProceso.Text);
            lstDuracionProceso.Items.Add(txtDuracionNuevoProceso.Text);


            // limpiar entradas de texto
            txtNombreNuevoProceso.Text = "";
            txtDuracionNuevoProceso.Text = "";

            // activar boton de iniciarProcesos
            btnIniciarProcesos.Enabled = true;
        }


        private void btnLimpiarCola_Click(object sender, EventArgs e)
        {
            // detiene timer
            timer.Enabled = false;

            // Elimina todos lo elementos de la cola 
            while (lstNumeroProceso.Items.Count > 0)
            {
                cola.Extraer();
                lstNumeroProceso.Items.RemoveAt(0);
                lstNombreProceso.Items.RemoveAt(0);
                lstDuracionProceso.Items.RemoveAt(0);
            }

            // permite la entrada de nuevos procesos
            txtDuracionNuevoProceso.Enabled = true;
            txtNombreNuevoProceso.Enabled = true;
            btnAgregarProceso.Enabled = true;
            btnIniciarProcesos.Enabled = true;
            btnPausarProcesos.Enabled = false;

            // reinicia contador de procesos y barra de progreso actual
            contadorProcesos = 0;
            barraActual = 0;

            // oculta todas etiquetas, reiniciar y ocultar las barras de progreso 
            lblProceso1.Visible = false;
            lblProceso2.Visible = false;
            lblProceso3.Visible = false;
            lblProceso4.Visible = false;
            lblProceso5.Visible = false;
            lblProceso6.Visible = false;
            lblProceso7.Visible = false;
            lblGlobal.Visible = false;

            pbr1.Visible = false;
            pbr2.Visible = false;
            pbr3.Visible = false;
            pbr4.Visible = false;
            pbr5.Visible = false;
            pbr6.Visible = false;
            pbr7.Visible = false;
            pgrGlobal.Visible = false;

            pbr1.Value = 0;
            pbr2.Value = 0;
            pbr3.Value = 0;
            pbr4.Value = 0;
            pbr5.Value = 0;
            pbr6.Value = 0;
            pbr7.Value = 0;
            pgrGlobal.Value = 0;

            // reiniciar tiempo y ocultar timer
            minutos = segundos = 0;
            lblTimer.Visible = false;
            iniciado = false;
            terminado = false;
            duracionGlobal = 0;

            // activar boton de iniciarProcesos
            btnIniciarProcesos.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblTimer.Text = "";
            btnPausarProcesos.Enabled = false;
            btnIniciarProcesos.Enabled = false;
        }

        private void btnIniciarProcesos_Click(object sender, EventArgs e)
        {
            // Bloquea la entrada de nuevos procesos o reinicio y permite pausar 
            txtDuracionNuevoProceso.Enabled = false;
            txtNombreNuevoProceso.Enabled = false;
            btnAgregarProceso.Enabled = false;

            btnIniciarProcesos.Enabled = false;
            btnPausarProcesos.Enabled = true;

            // checar que lista no esté vacía
            if (lstNumeroProceso.Items.Count == 0)
            {
                return;
            }

            // Inicialización o reanudación?
            if (iniciado)
            {
                timer.Enabled = true;
            }
            else
            {
                // activamos barra global
                pgrGlobal.Visible = true;
                lblGlobal.Visible = true;

                // asignamos su máximo como la suma de la duración de todos los procesos
                for (int i = 0; i < lstNumeroProceso.Items.Count; i++)
                {
                    duracionGlobal += Int32.Parse((string)lstDuracionProceso.Items[i]);
                }
                pgrGlobal.Maximum = duracionGlobal;


                // asignamos barras de progreso a la lista
                listaBarras.Add(pbr1);
                listaBarras.Add(pbr2);
                listaBarras.Add(pbr3);
                listaBarras.Add(pbr4);
                listaBarras.Add(pbr5);
                listaBarras.Add(pbr6);
                listaBarras.Add(pbr7);

                // asignamos labels a lista
                listaLabels.Add(lblProceso1);
                listaLabels.Add(lblProceso2);
                listaLabels.Add(lblProceso3);
                listaLabels.Add(lblProceso4);
                listaLabels.Add(lblProceso5);
                listaLabels.Add(lblProceso6);
                listaLabels.Add(lblProceso7);


                // determinamos qué proceso es el primero basado en SRT
                barraActual = cola.IndiceSRT();

                
                // Activamos etiqueta y barra de progreso del primer proceso basado en SRT
                listaLabels[barraActual].Text = (string)lstNombreProceso.Items[barraActual];
                listaLabels[barraActual].Visible = true;
                listaBarras[barraActual].Visible = true;

                // asigna la duraccion del proceso como maximo de la barra de progreso
                listaBarras[barraActual].Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);

                // Inicializa el timer
                timer.Enabled = true;
                lblTimer.Visible = true;
            }
            // Bandera de que ya se ha hecho la primera inicializacion 
            iniciado = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblBarraActual.Text = "Proceso en curso: " + (barraActual + 1);

            // procesos terminados?
            if (terminado)
            {
                return;
            }

            // formato del timer
            segundos++;
            if (segundos == 60)
            {
                segundos = 0;
                minutos++;
            }
            // mostrar datos del timer
            lblTimer.Text = String.Format("{0:00}:", minutos) + String.Format("{0:00}", segundos);

            /*
            // el quantum se completo? cambiamos proceso y reseteamos contadorQuantum
            if (contadorQuantum == quantum)
            {
                cambiarProceso();
            }

            // aumenta el contador de quantum
            contadorQuantum++;
            */

            // avanzamos en contador global
            if (pgrGlobal.Value < duracionGlobal)
            {
                pgrGlobal.Value++;
            }

            // la barra actual aun no llega a su fin? aumentamos 
            if(listaBarras[barraActual].Value < Int32.Parse((string)lstDuracionProceso.Items[barraActual]))
            {
                listaBarras[barraActual].Value++;
            }
            // intentamos cambiar de proceso
            else
            {
                // marcamos indice actual como terminado en la cola
                cola.ProcesoTerminado(barraActual);
                // obtenemos nuevo indice con SRT
                barraActual = cola.IndiceSRT();
                
                // Todos los procesos estan terminados? Detenemos el timer
                if (barraActual == -1)
                {
                    timer.Enabled = false;
                    MessageBox.Show("Cola de procesos terminada.");
                    return;
                }

                // activamos labels y progresBar del siguiente proceso y su  
                listaBarras[barraActual].Maximum = Int32.Parse((string)lstDuracionProceso.Items[barraActual]);
                listaLabels[barraActual].Text = (string)lstNombreProceso.Items[barraActual];
                listaBarras[barraActual].Visible = true;
                listaLabels[barraActual].Visible = true;

            }

        }

        private void btnPausarProcesos_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            btnPausarProcesos.Enabled = false;
            btnIniciarProcesos.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Enabled = false;
        }

    }
}