using System;
using System.Reflection;

namespace LYTest.VerifyService.WcfDynamic
{
    public class DynamicObject
    {
        private Type objType;
        private object obj;

        private BindingFlags CommonBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        public DynamicObject(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            this.obj = obj;
            objType = obj.GetType();
        }

        public DynamicObject(Type objType)
        {
            if (objType == null)
                throw new ArgumentNullException("objType");

            this.objType = objType;
        }

        public void CallConstructor()
        {
            CallConstructor(new Type[0], new object[0]);
        }

        public void CallConstructor(Type[] paramTypes, object[] paramValues)
        {
            ConstructorInfo ctor = objType.GetConstructor(paramTypes);
            if (ctor == null)
            {
                throw new DynamicProxyException(
                        Constants.ErrorMessages.ProxyCtorNotFound);
            }

            obj = ctor.Invoke(paramValues);
        }

        public object GetProperty(string property)
        {
            object retval = objType.InvokeMember(
                property,
                BindingFlags.GetProperty | CommonBindingFlags,
                null /* Binder */,
                obj,
                null /* args */);

            return retval;
        }

        public object SetProperty(string property, object value)
        {
            object retval = objType.InvokeMember(
                property,
                BindingFlags.SetProperty | CommonBindingFlags,
                null /* Binder */,
                obj,
                new object[] { value });

            return retval;
        }

        public object GetField(string field)
        {
            object retval = objType.InvokeMember(
                field,
                BindingFlags.GetField | CommonBindingFlags,
                null /* Binder */,
                obj,
                null /* args */);

            return retval;
        }

        public object SetField(string field, object value)
        {
            object retval = objType.InvokeMember(
                field,
                BindingFlags.SetField | CommonBindingFlags,
                null /* Binder */,
                obj,
                new object[] { value });

            return retval;
        }

        public object CallMethod(string method, params object[] parameters)
        {
            object retval = objType.InvokeMember(
                 method,
                 BindingFlags.InvokeMethod | CommonBindingFlags,
                 null /* Binder */,
                 obj,
                 parameters /* args */);

            return retval;
        }

        public object CallMethod(string method, Type[] types,
            object[] parameters)
        {
            if (types.Length != parameters.Length)
                throw new ArgumentException(
                    Constants.ErrorMessages.ParameterValueMistmatch);

            MethodInfo mi = objType.GetMethod(method, types);
            if (mi == null)
                throw new ApplicationException(string.Format(
                    Constants.ErrorMessages.MethodNotFound, method));

            object retval = mi.Invoke(obj, CommonBindingFlags, null,
                parameters, null);

            return retval;
        }

        public Type ObjectType
        {
            get
            {
                return objType;
            }
        }

        public object ObjectInstance
        {
            get
            {
                return obj;
            }
        }

        public BindingFlags BindingFlags
        {
            get
            {
                return CommonBindingFlags;
            }

            set
            {
                CommonBindingFlags = value;
            }
        }
    }
}
