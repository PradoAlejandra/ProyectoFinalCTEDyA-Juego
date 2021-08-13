
using System;
using System.Collections.Generic;
namespace DeepSpace
{

	class Estrategia
	{
		
		
		public String Consulta1( ArbolGeneral<Planeta> arbol)
		{	
			int nivel=0;
			
			Cola<ArbolGeneral<Planeta>> c= new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			
			c.encolar(arbol);
			c.encolar(null);
			
			while(!c.esVacia()){
				
				arbolAux=c.desencolar();
				
				if(arbolAux!=null){
					
					if(arbolAux.getDatoRaiz().EsPlanetaDeLaIA())
						return "Distancia desde la raíz a al planeta más cercano perteneciente a la IA: "+nivel;
					
					foreach(var hijo in arbolAux.getHijos())
						c.encolar(hijo);
					
				}
				
				else{
					if(!c.esVacia()){
						nivel++;
						c.encolar(null);
					}
				}
			}
			
			return "";
		}


		public String Consulta2( ArbolGeneral<Planeta> arbol)
		{
			string msj="Cantidad de planetas con población mayor a 10:";
			int nivel=0,cont=0;
			
			Cola<ArbolGeneral<Planeta>> c = new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			
			c.encolar(arbol);
			c.encolar(null);
			
			while(!c.esVacia()){
				
				arbolAux=c.desencolar();
				
				if(arbolAux!=null){
					
					if(arbolAux.getDatoRaiz().Poblacion()>10)
						cont++;
					
					foreach(var hijo in arbolAux.getHijos())
						c.encolar(hijo);
				}
				
				else{
						msj+="\nNivel "+nivel+" --> "+cont;
						nivel++;
						cont=0;
						if(!c.esVacia())
							c.encolar(null);
					
				}
			}
			return msj;
		}


		public String Consulta3( ArbolGeneral<Planeta> arbol)
		{
			string msj="Promedio poblacional:";
			int nivel=0, acPlanetas=0, acPoblacion=0;
			
			Cola<ArbolGeneral<Planeta>> c= new Cola<ArbolGeneral<Planeta>>();
			ArbolGeneral<Planeta> arbolAux;
			
			c.encolar(arbol);
			c.encolar(null);
			
			while(!c.esVacia()){
				arbolAux=c.desencolar();
				
				if(arbolAux!=null){
					acPlanetas++;
					acPoblacion+=arbolAux.getDatoRaiz().Poblacion();
					
					foreach(var hijo in arbolAux.getHijos())
						c.encolar(hijo);
				}
				
				else{
					int prom=acPoblacion/acPlanetas;
					msj+="\nNivel "+nivel+" --> "+prom;
					nivel++;
					acPlanetas=0;
					acPoblacion=0;
					if(!c.esVacia())
						c.encolar(null);
				}
				
			}
			
			return msj;
		}
		
		
		public Movimiento CalcularMovimiento(ArbolGeneral<Planeta> arbol)
		{
			//Si la raíz no es de la IA
			if(!arbol.getDatoRaiz().EsPlanetaDeLaIA()){
				
				//Camino (lista) desde la raíz a la IA más cercana
				List<ArbolGeneral<Planeta>> caminoHaciaRaiz= caminoARaiz(arbol, new List<ArbolGeneral<Planeta>>());
				
				//Se determina cuál es el arbol que contiene al planeta de la IA más cercano a la raíz (el último de la lista)
				ArbolGeneral<Planeta> ArbolMasCercanoRaiz= caminoHaciaRaiz[caminoHaciaRaiz.Count-1];
				
				//Lista de aliados descendientes de la IA más cercana a la raíz (la que debería realizar el movimiento hacía la raíz)
				List<ArbolGeneral<Planeta>> planetasAliados= aliadosDescendientes(ArbolMasCercanoRaiz, new List<ArbolGeneral<Planeta>>());
				
				//Si el planeta de la IA más cercano a la raíz tiene aliados
				if(planetasAliados.Count>1){
					
					//Se determina en que posición de la lista se encuentra el planeta aliado más poblado
					int ind= indiceMayorPoblacion(planetasAliados);
					
					//Se aisla al planeta aliado con mayor población
					Planeta aliadoMasPoblado= planetasAliados[ind].getDatoRaiz();
					
					//Si el mayor de los planetas aliados tiene más población que el planeta de la IA más cercano a la raíz
					if(aliadoMasPoblado.Poblacion()>ArbolMasCercanoRaiz.getDatoRaiz().Poblacion()){
						
						Planeta origen=planetasAliados[ind].getDatoRaiz(), destino=planetasAliados[ind-1].getDatoRaiz();
					
						//Se retorna como movimiento origen->planeta aliadoMayor / destino->su siguiente (-1)
						return new Movimiento(origen,destino);
					}
					
					
					//Si el mayor de los planetas aliados no tiene más población que el planeta de la IA más cercano a la raíz
					else{
						
						Planeta origen=caminoHaciaRaiz[caminoHaciaRaiz.Count-1].getDatoRaiz(), destino=caminoHaciaRaiz[caminoHaciaRaiz.Count-2].getDatoRaiz();
						
						//Retornar como movimiento origen->planeta de la IA más cercano a la raíz / destino-> su siguiente(-1)
						return new Movimiento(origen,destino);
					}
				}
									
				//Si el planeta de la IA más cercano a la raíz no tiene aliados
				else{
					
					Planeta origen=caminoHaciaRaiz[caminoHaciaRaiz.Count-1].getDatoRaiz(), destino=caminoHaciaRaiz[caminoHaciaRaiz.Count-2].getDatoRaiz();
				
					//Se retorna como movimiento origen->planeta de la IA más cercano a la raíz / destino-> su siguiente´
					return new Movimiento(origen,destino);
					
				}
				
			}
			
			
			//Si la raíz es de la IA
			if(arbol.getDatoRaiz().EsPlanetaDeLaIA()){
				
				//Se crea el camino desde la raíz al enemigo más cercano
				List<ArbolGeneral<Planeta>> camino= caminoAEnemigo(arbol, new List<ArbolGeneral<Planeta>>());
				
				//Se determina cuál es el indice del árbol donde está el planeta de la IA con el que se va a atacar
				int indxUltimoIA=-1;
				for(int i=0;i<camino.Count;i++){
					if(camino[i].getDatoRaiz().EsPlanetaDeLaIA())
						indxUltimoIA=i;
				}
				
				//Searbol aisla el arbol donde se encuentra el planeta atacante
				ArbolGeneral<Planeta> PlanetaAtacante= camino[indxUltimoIA];
				
				//Se crea la lista con los arboles ancestros aliados del arbol atacante
				List<ArbolGeneral<Planeta>> lisAliados= aliadosAncestros(camino,indxUltimoIA);
				
				//Si la lista de aliados es mayor a uno (no es solo la raíz)
				if(lisAliados.Count>1){
				
					//Se determina en que posición de la lista se encuentra el arbol con el planeta aliado más poblado
					int ind= indiceMayorPoblacion(lisAliados);
					
					//Se aisla al arbol con el planeta aliado más poblado
					ArbolGeneral<Planeta> arbolAliadoMasPoblado= lisAliados[ind];
					
					
					//Si el aliado tiene mayor poblacion que el planeta atacante
					if(arbolAliadoMasPoblado.getDatoRaiz().Poblacion()>PlanetaAtacante.getDatoRaiz().Poblacion()){ //arbolPlantaAtacante.getDatoRaiz().Poblacion())
					
					
						//Se retorna como movimiento origen -> aliado mas poblado / destino-> su siguiente +1
						Planeta origen=lisAliados[ind].getDatoRaiz(), destino=lisAliados[ind+1].getDatoRaiz();
					
						return new Movimiento(origen,destino);
					}
					
					
					//Si el aliado no tiene mayor poblacion que el planeta atacante
					else{
					
						//Se retorna como moviniento origen->planeta atacante / destino->su siguiente+1
						Planeta origen=camino[indxUltimoIA].getDatoRaiz(), destino=camino[indxUltimoIA+1].getDatoRaiz();
				
						return new Movimiento(origen,destino);
					
					}
				}
				
				
				//Si la lista de aliados no es mayor a uno (solo la raiz)
				else{
				
					//Se retorna como movimiento origen->planetaatacante / destino->su siguiente+1
					Planeta origen=camino[indxUltimoIA].getDatoRaiz(), destino=camino[indxUltimoIA+1].getDatoRaiz();
				
					return new Movimiento(origen,destino);
					
				}
				
			}
			
			
			return null;
		}
		
		
		
		private List<ArbolGeneral<Planeta>> caminoARaiz(ArbolGeneral<Planeta> arbol, List<ArbolGeneral<Planeta>> camino){
			
			camino.Add(arbol);
			
			//Si el el arbol agregado a la lista contiene un planeta perteneciente a la IA
			if(arbol.getDatoRaiz().EsPlanetaDeLaIA())
				return camino;
			
			//Si no pertenece
			else{
				//Procesa cada hijo
				foreach(var hijo in arbol.getHijos()){
					List<ArbolGeneral<Planeta>> caminoAux= caminoARaiz(hijo,camino);//usa la función recursiva con cada hijo, esta llamda puede devolver una lista con el camino o null
					
					if(caminoAux!=null)//Si la llamada no devolvió null (una lista con el camino)
						return caminoAux;//Retorna la lista
				}
				
				camino.RemoveAt(camino.Count-1);//Si llega a una hoja o si recorre por completo los hijos de un árbol no entra al foreach, saca el ultimo elemento
				
			}
			
			return null;// devuelve null
			
		}
		
		
		
		private List<ArbolGeneral<Planeta>> caminoAEnemigo(ArbolGeneral<Planeta> arbol, List<ArbolGeneral<Planeta>> camino){
			
			//Añade a la lista el planeta contenido en la raíz del arbol
			camino.Add(arbol);
			
			if(arbol.getDatoRaiz().EsPlanetaDelJugador())//Si el planeta añadido a la lista es del jugador
				return camino;//Retorna la lista
			
			
			foreach(var hijo in arbol.getHijos()){//A cada hijo
				
				List<ArbolGeneral<Planeta>> caminoAux= caminoAEnemigo(hijo,camino);//Se lo procesa recurivamente y el resultado de esa llamada se guarda en caminoAux
				
				if(caminoAux!=null)//Si caminoAux es distinto de null
					return caminoAux;//Retorna caminoAux
				
			}
			
			camino.RemoveAt(camino.Count-1);//Si no se ejecuta el foreach significa que se llegó a una hoja o recorrió todos los hijos del árbol y aún no se encontro el camino, saca el último elemento
			
			return null;//Retorna null
			
		}
		
		
		
		private List<ArbolGeneral<Planeta>> aliadosDescendientes(ArbolGeneral<Planeta> arbol, List<ArbolGeneral<Planeta>> lisAliados){
			//Se agrega el arbol a la lista (Al árbol pasado como primer parámetro se le busca alidos entre sus descendientes)
			lisAliados.Add(arbol);
			
			//Se recorre cada uno de sus hijos 
			foreach(var hijo in arbol.getHijos()){
				if(hijo.getDatoRaiz().EsPlanetaDeLaIA()==false)//Si el hijo no contiene un planeta perteneciente a la IA
					continue;//Continua procesando el siguiente hijo
				
				else{//Si pertenece a la IA
					List<ArbolGeneral<Planeta>> aliadosAux= aliadosDescendientes(hijo,lisAliados);//Se llama recursivamente al mismo método pasandole como parametro el subarbol hijo y la lista de aliados existente hasta ese momento. El resultado de esta llamada se guarda en una list auxiliar
					lisAliados=aliadosAux;//El contenido de la lista auxiliar se guarda en la lista aliados
				}
			}
			
			return lisAliados;//Se retorna la lista de aliados
		}
		
		private int indiceMayorPoblacion(List<ArbolGeneral<Planeta>> lista){//Devuele el indice del planeta con mayor población en la lista
			int mayor=0, ind=-1;
			
			for(int i=0;i<lista.Count;i++){
				if(lista[i].getDatoRaiz().Poblacion()>mayor){
					mayor= lista[i].getDatoRaiz().Poblacion();
					ind=i;
				}
			}
			
			return ind;
		}
		
		
		private List<ArbolGeneral<Planeta>> aliadosAncestros(List<ArbolGeneral<Planeta>> camino, int limite){
			
			List<ArbolGeneral<Planeta>> aliados= new List<ArbolGeneral<Planeta>>();
			
			for(int i=0;i<=limite;i++){
				if(camino[i].getDatoRaiz().EsPlanetaDeLaIA())
					aliados.Add(camino[i]);
			}
			
			return aliados;
		}
		
	}
}
