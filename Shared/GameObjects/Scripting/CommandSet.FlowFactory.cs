using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    public partial class CommandSet
    {
        /// <summary>
        /// Classe permettant de créer des structures de contrôles.
        /// </summary>
        public static class FlowFactory
        {
            public enum FlowFactorySpecial
            {
                If, Else, Elsif
            }
            
            /// <summary>
            /// Représente un marqueur "else" dans une structure "if".
            /// </summary>
            public static FlowFactorySpecial Else = FlowFactorySpecial.Else;
            /// <summary>
            /// Représente un marqueur "else if" dans une structure "if".
            /// </summary>
            public static FlowFactorySpecial Elsif = FlowFactorySpecial.Elsif;
            /// <summary>
            /// Crée une structure "If" à partir des paramètres spécifiés, qui sont :
            /// - Une condition suivi d'un ou plusieurs IExecutable.
            /// - [Optionnel] Elsif suivi d'un ou plusieurs IExecutable.
            /// - [Optionnel] Else suivi d'un ou plusieurs IExecutable
            /// </summary>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static IfAction If(params object[] parameters)
            {
                IfAction mainIf = new IfAction();
                IfAction currentIf = mainIf;
                ActionCollection currentCollection = new ActionCollection();
                mainIf.Cond = (Condition)parameters[0];
                bool hasElse = false;
                for(int i = 1; i < parameters.Count(); i++)
                {
                    object param = parameters[i];
                    if (param is IExecutable)
                    {
                        currentCollection.Add((IExecutable)param);
                    }

                    if (param is FlowFactorySpecial)
                    {
                        FlowFactorySpecial spec = (FlowFactorySpecial)param;
                        if (spec == FlowFactorySpecial.Else)
                        {
                            currentIf.TrueAction = currentCollection;
                            currentCollection = new ActionCollection();
                            hasElse = true;
                        }
                        else if (spec == FlowFactorySpecial.Elsif)
                        {
                            IfAction newIf = new IfAction();
                            currentIf.TrueAction = currentCollection;
                            currentIf.FalseAction = newIf;
                            currentCollection = new ActionCollection();
                            currentIf = newIf;
                            hasElse = true;
                        }
                    }

                    if (param is Condition)
                    {
                        currentIf.Cond = (Condition)param;
                    }
                }
                if (hasElse)
                    currentIf.FalseAction = currentCollection;
                else
                    currentIf.TrueAction = currentCollection;
                return mainIf;
            }
        }
    }
}
