using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace MesaTangible.Tests
{
    public class TagDatabaseTests
    {
        private TagDatabase tagDatabase;

        [SetUp]
        public void Setup()
        {
            var go = new GameObject("TagDatabase");
            tagDatabase = go.AddComponent<TagDatabase>();
        }

        [TearDown]
        public void TearDown()
        {
            if (tagDatabase != null)
            {
                Object.DestroyImmediate(tagDatabase.gameObject);
            }
        }

        [Test]
        public void GetTipo_QRIdValido_ReturnsRouter()
        {
            var tipo = tagDatabase.GetTipo("ROUTER");
            Assert.AreEqual(DiscoTipo.Router, tipo);
        }

        [Test]
        public void GetTipo_QRIdValido_ReturnsSwitch()
        {
            var tipo = tagDatabase.GetTipo("SWITCH");
            Assert.AreEqual(DiscoTipo.Switch, tipo);
        }

        [Test]
        public void GetTipo_QRIdValido_ReturnsPC()
        {
            var tipo = tagDatabase.GetTipo("PC-A");
            Assert.AreEqual(DiscoTipo.PC, tipo);
        }

        [Test]
        public void GetTipo_QRIdDesconocido_ReturnsDesconocido()
        {
            var tipo = tagDatabase.GetTipo("INVALIDO");
            Assert.AreEqual(DiscoTipo.Desconocido, tipo);
        }

        [Test]
        public void GetTipo_QRIdNull_ReturnsDesconocido()
        {
            var tipo = tagDatabase.GetTipo(null);
            Assert.AreEqual(DiscoTipo.Desconocido, tipo);
        }

        [Test]
        public void GetTipo_QRIdVacio_ReturnsDesconocido()
        {
            var tipo = tagDatabase.GetTipo("");
            Assert.AreEqual(DiscoTipo.Desconocido, tipo);
        }

        [Test]
        public void GetTipo_QRIdMinusculas_ReturnsTipo()
        {
            var tipo = tagDatabase.GetTipo("router");
            Assert.AreEqual(DiscoTipo.Router, tipo);
        }

        [Test]
        public void TryGetDisco_QRIdValido_ReturnsTrue()
        {
            bool resultado = tagDatabase.TryGetDisco("ROUTER", out var disco);
            Assert.IsTrue(resultado);
            Assert.IsNotNull(disco);
            Assert.AreEqual("ROUTER", disco.qrId);
        }

        [Test]
        public void TryGetDisco_QRIdInvalido_ReturnsFalse()
        {
            bool resultado = tagDatabase.TryGetDisco("INVALIDO", out var disco);
            Assert.IsFalse(resultado);
            Assert.IsNull(disco);
        }

        [Test]
        public void ObtenerTodos_Returns14Discos()
        {
            var discos = tagDatabase.ObtenerTodos();
            Assert.AreEqual(14, discos.Count);
        }

        [Test]
        public void AgregarDisco_NuevoDisco_Agregado()
        {
            tagDatabase.AgregarDisco("TEST-DISCO", DiscoTipo.PC, "Test", Color.white);
            
            var tipo = tagDatabase.GetTipo("TEST-DISCO");
            Assert.AreEqual(DiscoTipo.PC, tipo);
        }

        [Test]
        public void AgregarDisco_DiscoExistente_NoDuplica()
        {
            tagDatabase.AgregarDisco("ROUTER", DiscoTipo.PC, "Test", Color.white);
            
            var discos = tagDatabase.ObtenerTodos();
            Assert.AreEqual(14, discos.Count);
        }
    }
}