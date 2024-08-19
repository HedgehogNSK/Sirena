namespace Hedgey.Extensions.Types;

static public class TypesExtension{

  static public IEnumerable<Type> GetBaseTypes(this Type type)
  {
    if(type==null) yield break;
    Type? current = type.BaseType;
    while (current != null)
    {
      yield return current;
      current = current.BaseType;
    }
  }

  static public bool HasParent(this Type type, Type parent){
    var baseTypes =  type.GetBaseTypes();
    if(!parent.IsGenericType)
      return baseTypes.Any(_type => _type == parent);

    return baseTypes.Any(_type => _type.IsGenericType &&  _type.GetGenericTypeDefinition() == parent);
  }
}