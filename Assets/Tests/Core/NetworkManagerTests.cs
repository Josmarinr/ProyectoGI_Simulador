using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace MesaTangible.Tests
{
    public class NetworkManagerTests
    {
        private GameObject go;
        private NetworkManager networkManager;
        private TagDatabase tagDatabase;

        [SetUp]
        public void Setup()
        {
            go = new GameObject("TestGameObject");
            tagDatabase = go.AddComponent<TagDatabase>();
            networkManager = go.AddComponent<NetworkManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void AgregarNodo_QRIdValido_NodoAgregado()
        {
            var id = networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            
            Assert.IsNotNull(id);
            Assert.AreEqual("ROUTER", id);
        }

        [Test]
        public void AgregarNodo_QRIdInvalido_Null()
        {
            var id = networkManager.AgregarNodo("INVALIDO", Vector3.zero, Quaternion.identity);
            
            Assert.IsNull(id);
        }

        [Test]
        public void AgregarNodo_MismoQRId_ActualizaPosicion()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("ROUTER", new Vector3(1, 0, 0), Quaternion.identity);
            
            var nodos = networkManager.ObtenerNodos();
            Assert.AreEqual(1, nodos.Count);
        }

        [Test]
        public void ObtenerNodos_SinNodos_ListaVacia()
        {
            var nodos = networkManager.ObtenerNodos();
            Assert.IsEmpty(nodos);
        }

        [Test]
        public void ObtenerNodos_ConNodos_ListaConNodos()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", new Vector3(1, 0, 0), Quaternion.identity);
            
            var nodos = networkManager.ObtenerNodos();
            Assert.AreEqual(2, nodos.Count);
        }

        [Test]
        public void RemoverNodo_NodoExistente_NodoRemovido()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.RemoverNodo("ROUTER");
            
            var nodos = networkManager.ObtenerNodos();
            Assert.IsEmpty(nodos);
        }

        [Test]
        public void RemoverNodo_NodoNoExistente_NoException()
        {
            Assert.DoesNotThrow(() => networkManager.RemoverNodo("NOEXISTE"));
        }

        [Test]
        public void HayCamino_MismoNodo_True()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            
            bool hayCamino = networkManager.HayCamino("ROUTER", "ROUTER");
            Assert.IsTrue(hayCamino);
        }

        [Test]
        public void HayCamino_NodosNoConectados_False()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", new Vector3(10, 0, 0), Quaternion.identity);
            
            bool hayCamino = networkManager.HayCamino("ROUTER", "PC-A");
            Assert.IsFalse(hayCamino);
        }

        [Test]
        public void AgregarConexion_DosNodos_ConexionCreada()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", Vector3.zero, Quaternion.identity);
            networkManager.AgregarConexion("ROUTER", "PC-A");
            
            var conexiones = networkManager.ObtenerConexiones();
            Assert.AreEqual(1, conexiones.Count);
        }

        [Test]
        public void HayCamino_NodosConectados_True()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", Vector3.zero, Quaternion.identity);
            networkManager.AgregarConexion("ROUTER", "PC-A");
            
            bool hayCamino = networkManager.HayCamino("ROUTER", "PC-A");
            Assert.IsTrue(hayCamino);
        }

        [Test]
        public void HayCamino_TresNodosEnLinea_True()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("SWITCH", new Vector3(1, 0, 0), Quaternion.identity);
            networkManager.AgregarNodo("PC-A", new Vector3(2, 0, 0), Quaternion.identity);
            networkManager.AgregarConexion("ROUTER", "SWITCH");
            networkManager.AgregarConexion("SWITCH", "PC-A");
            
            bool hayCamino = networkManager.HayCamino("ROUTER", "PC-A");
            Assert.IsTrue(hayCamino);
        }

        [Test]
        public void HayCamino_TopologiaMalla_True()
        {
            networkManager.AgregarNodo("ROUTER-1", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("ROUTER-2", new Vector3(1, 0, 0), Quaternion.identity);
            networkManager.AgregarNodo("PC-A", new Vector3(2, 0, 0), Quaternion.identity);
            networkManager.AgregarConexion("ROUTER-1", "ROUTER-2");
            networkManager.AgregarConexion("ROUTER-1", "PC-A");
            networkManager.AgregarConexion("ROUTER-2", "PC-A");
            
            bool hayCamino = networkManager.HayCamino("ROUTER-1", "PC-A");
            Assert.IsTrue(hayCamino);
        }

        [Test]
        public void ObtenerNodo_NodoExistente_NodoRetornado()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            
            var nodo = networkManager.ObtenerNodo("ROUTER");
            Assert.IsNotNull(nodo);
            Assert.AreEqual("ROUTER", nodo.qrId);
        }

        [Test]
        public void ObtenerNodo_NodoNoExistente_Null()
        {
            var nodo = networkManager.ObtenerNodo("NOEXISTE");
            Assert.IsNull(nodo);
        }

        [Test]
        public void AgregarEntradaRouting_EntradaAgregada()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarEntradaRouting("ROUTER", "192.168.1.0/24", "PC-A");
            
            var nodo = networkManager.ObtenerNodo("ROUTER");
            Assert.IsTrue(nodo.tablaRouting.ContainsKey("192.168.1.0/24"));
            Assert.AreEqual("PC-A", nodo.tablaRouting["192.168.1.0/24"]);
        }

        [Test]
        public void ObtenerSiguienteSaldo_SaltoRetornado()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", Vector3.zero, Quaternion.identity);
            networkManager.AgregarEntradaRouting("ROUTER", "192.168.1.0/24", "PC-A");
            
            var siguienteSalto = networkManager.ObtenerSiguienteSalto("ROUTER", "192.168.1.0/24");
            Assert.AreEqual("PC-A", siguienteSalto);
        }

        [Test]
        public void Limpiar_TodoLimpiado()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", Vector3.zero, Quaternion.identity);
            
            networkManager.Limpiar();
            
            var nodos = networkManager.ObtenerNodos();
            Assert.IsEmpty(nodos);
        }

        [Test]
        public void EstaConectado_NodoSinConexiones_False()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            
            bool conectado = networkManager.EstaConectado("ROUTER");
            Assert.IsFalse(conectado);
        }

        [Test]
        public void EstaConectado_NodoConConexiones_True()
        {
            networkManager.AgregarNodo("ROUTER", Vector3.zero, Quaternion.identity);
            networkManager.AgregarNodo("PC-A", Vector3.zero, Quaternion.identity);
            networkManager.AgregarConexion("ROUTER", "PC-A");
            
            bool conectado = networkManager.EstaConectado("ROUTER");
            Assert.IsTrue(conectado);
        }
    }
}