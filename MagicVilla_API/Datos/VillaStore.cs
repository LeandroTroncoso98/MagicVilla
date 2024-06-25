using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList= new List<VillaDto>()
        {
            new VillaDto{Id = 1, Nombre = "Vista a la piscina", Ocupantes = 3, MetrosCuadrados = 50 },
            new VillaDto{Id = 2, Nombre = "Vista a la playa",Ocupantes = 4, MetrosCuadrados = 100}
        };
    }
}
