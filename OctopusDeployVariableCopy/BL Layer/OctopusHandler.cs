using Octopus.Client.Model;
using OctopusDeployVariableCopy.OctopusAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctopusDeployVariableCopy.BL_Layer
{
    class OctopusHandler
    {
        private OctopusAccessor _octoDal;

        public OctopusHandler(string server, string apiKey)
        {
            _octoDal = new OctopusAccessor(server, apiKey);
        }

        public void CopyVariableSet(string varSetName, string newVarSetName, int numberOfCopies = 1, bool keepTheScope = true)
        {
            try
            {
                var originalLibVarSet = _octoDal.GetLibraryVariableSetByName(varSetName);
                var originalVariables = _octoDal.GetVariableSet(originalLibVarSet.VariableSetId).Variables;
                var emptyScope = new ScopeSpecification();
                
                string copiedNumber = null;
                for (int i = 0; i < numberOfCopies; i++)
                {
                    if (i > 0)
                    {
                        copiedNumber = i.ToString();
                    }
                    var newVarSet = _octoDal.CreateNewLibraryVariableSetWithVariableSet($"{newVarSetName}{copiedNumber}", originalLibVarSet.Description);
                    foreach (var variable in originalVariables)
                    {
                        if(keepTheScope)
                        {
                            _octoDal.AddVariableToVariableSet(newVarSet.VariableSetId, variable.Name, variable.Value, variable.Scope, variable.IsEditable, variable.IsSensitive, variable.Prompt);
                        }
                        else
                        {
                            _octoDal.AddVariableToVariableSet(newVarSet.VariableSetId, variable.Name, variable.Value, emptyScope, variable.IsEditable, variable.IsSensitive, variable.Prompt);
                        }

                    }

                }
            }
            catch(Exception e)
            {
                throw new Exception("Failed to copy the variable set", e);
            }
        }

        public Dictionary<string, string> GetAllLibraryVariableSetsAvaialable()
        {
            try
            {
                var result = new Dictionary<string, string>();
                var libVarSets = _octoDal.GetLibraryVariableSets();
                foreach (var libVarSet in libVarSets)
                {
                    result.Add(libVarSet.Name, libVarSet.Id);
                }
                return result;
            }
            catch(Exception e)
            {
                throw new Exception("Failed to get all of the library variable sets", e);
            }

        }
    }
}
