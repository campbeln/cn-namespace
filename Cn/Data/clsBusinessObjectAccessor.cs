using System;
using System.Collections;
using System.Collections.Generic;

namespace Cn.Data {


	public class ParameterizedProperty<typeReturn> {
		private typeReturn[] oReturn;

		public ParameterizedProperty(typeReturn[] oSource) {
			oReturn = oSource;
		}

		public typeReturn this[int iIndexer] {
			get { return oReturn[iIndexer]; }
			set { oReturn[iIndexer] = value; }
		}
	}




	public enum enumOperator {
		Equals = 1,
		NotEquals = 2
	}


	public interface IBusinessObjectAccessor {
		List<object> this[string sPropertyName, List<Criteria> l_oCriteria] { get; }
		object this[string sPropertyName] { get; set; }
		object this[string sPropertyName, object oIndexer] { get; set; }
		bool __IsDirty { get; set; }
	}


	public class Criteria {
		public object Value { get; set; }
		public string PropertyName { get; set; }
		public enumOperator ComparisonOperator { get; set; }
		
		public Criteria() {}
		
		public Criteria(string sPropertyName, object oValue, enumOperator eComparisonOperator) {
			PropertyName = sPropertyName;
			Value = oValue;
			ComparisonOperator = eComparisonOperator;
		}

	} //# public class Criteria


	public class BusinessObjectAccessor {

		public static object IndexerImplimentation(IBusinessObjectAccessor oBusinessObject, bool bSet, object oValue, string sPropertyName) {
				//#### 
			if (bSet) {
				bool bSuccess = Mirror.SetPropertyValue(oBusinessObject, sPropertyName, oValue);

//				oBusinessObject.__IsDirty = (oBusinessObject.__IsDirty || oValue != Mirror.GetPropertyValue(oBusinessObject, sPropertyName));
				oBusinessObject.__IsDirty = (oBusinessObject.__IsDirty || bSuccess);

				return bSuccess;
			}
				//#### 
			else {
				return Mirror.GetPropertyValue(oBusinessObject, sPropertyName);
			}
		}

		public static object IndexerImplimentation(IBusinessObjectAccessor oBusinessObject, bool bSet, object oValue, string sPropertyName, object oIndexer) {
			object oPropertyValue = Mirror.GetPropertyValue(oBusinessObject, sPropertyName);
			IDictionary oDictionary = oPropertyValue as IDictionary;
			IList oArray = oPropertyValue as IList;
			int iIndex = oIndexer as int? ?? -1;

			try {
					//#### 
				if (oDictionary != null) {
					if (bSet)	{ oDictionary[oIndexer] = oValue; }
					else		{ oPropertyValue = oDictionary[oIndexer]; }
				}
					//#### 
				else if (oArray != null && iIndex > -1 && iIndex < oArray.Count) {
					if (bSet)	{ oArray[iIndex] = oValue; }
					else		{ oPropertyValue = oArray[iIndex]; }
				}
					//#### 
				else {
					throw new NotSupportedException("'" + sPropertyName + "' is not of a supported indexed object type.");
				}
			}
			catch (Exception oEx) {
					//#### 
				throw oEx;
			}

			return oPropertyValue;
		}

		public static List<object> IndexerImplimentation(IBusinessObjectAccessor oBusinessObject, string sPropertyName, List<Criteria> l_oCriteria) {
			List<object> l_oReturn = new List<object>();
			object oPropertyValue = Mirror.GetPropertyValue(oBusinessObject, sPropertyName);
			object oSubPropertyValue = null;
			IEnumerable oArray = oPropertyValue as IEnumerable;
			int i;
			bool bCurrentIsValid = false;

//l_oCriteria.FindAll()
//Predicate<IBusinessObjectAccessor> o = new Predicate<IBusinessObjectAccessor>();


			if (oArray == null) {
				for (i = 0; i < l_oCriteria.Count; i++) {
					oSubPropertyValue = Mirror.GetPropertyValue(oPropertyValue, l_oCriteria[i].PropertyName);
					
					switch (l_oCriteria[i].ComparisonOperator) {
						case enumOperator.Equals: {
							bCurrentIsValid = (oSubPropertyValue == l_oCriteria[i].Value);
							break;
						}
						case enumOperator.NotEquals: {
							bCurrentIsValid = (oSubPropertyValue != l_oCriteria[i].Value);
							break;
						}
					}
				}

				if (bCurrentIsValid) {
					l_oReturn.Add(oSubPropertyValue);
				}
			}
			else {
				IEnumerator oEnumerator = oArray.GetEnumerator();
			}

return l_oReturn;
		}


	} //# public class BusinessObjectAccessor

}
