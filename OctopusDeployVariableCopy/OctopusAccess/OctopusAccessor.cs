using Octopus.Client;
using Octopus.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctopusDeployVariableCopy.OctopusAccess
{
    public class OctopusAccessor
    {
        private string _apiKey;
        private string _server;

        private OctopusServerEndpoint _endPoint;
        private OctopusRepository _repository;

        public OctopusAccessor(string server, string apiKey)
        {
            _apiKey = apiKey;
            _server = server;

            _endPoint = new OctopusServerEndpoint(_server, _apiKey);
            _repository = new OctopusRepository(_endPoint);
        }

        public List<LibraryVariableSetResource> GetLibraryVariableSets()
        {
            return _repository.LibraryVariableSets.FindAll();
        }

        public LibraryVariableSetResource GetLibraryVariableSetByName(string name)
        {
            Func<LibraryVariableSetResource, bool> searchMethod = delegate(LibraryVariableSetResource libVarSet)
            {
                if (libVarSet.Name == name)
                {
                    return true;
                }
                return false;
            };

            return _repository.LibraryVariableSets.FindOne(searchMethod);
        }

        public VariableSetResource GetVariableSet(string variableSetId)
        {
            return _repository.VariableSets.Get(variableSetId);
        }

        /// <summary>
        /// Creates new library variable set and also attaches a new variable set to it, and returns the new library variable set
        /// </summary>
        /// <param name="variableSetName"></param>
        /// <returns></returns>
        public LibraryVariableSetResource CreateNewLibraryVariableSetWithVariableSet(string variableSetName, string description="")
        {
            LibraryVariableSetResource newLibVarSet = new LibraryVariableSetResource()
            {
                Name = variableSetName,
                ContentType = VariableSetContentType.Variables,
                Description = description
            };
            var result = _repository.LibraryVariableSets.Create(newLibVarSet);
            return result;
        }

        public void AddVariableToVariableSet(string varSetId, string varName, string varValue, ScopeSpecification scope, bool isEditable = true, bool isSensitive = false, VariablePromptOptions promptOptions = null)
        {
            VariableResource variable = new VariableResource()
            {
                Name = varName,
                Value = varValue,
                Scope = scope,
                IsEditable = isEditable,
                IsSensitive = isSensitive,
                Prompt = promptOptions
            };

            var varSet = _repository.VariableSets.Get(varSetId);
            varSet.Variables.Add(variable);
            _repository.VariableSets.Modify(varSet);
        }
        
    }
}
