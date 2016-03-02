﻿
CsLuaFramework.Wrapping.Wrapper = _M.NE({[0] = function(interactionElement, generics, staticValues)
    local baseTypeObject, members = System.Object.__meta(staticValues);
    local typeObject = System.Type('CsLuaFramework.Wrapping','Wrapper',baseTypeObject,0,nil,nil,interactionElement);

    local methodGenericsMapping = {['T'] = 1};
    local methodGenerics = _M.MG(methodGenericsMapping);

    _M.IM(members,'Wrap',{
        level = typeObject.Level,
        memberType = 'Method',
        scope = 'Public',
        static = false,
        types = {System.String.__typeof},
        generics = methodGenericsMapping,
        func = function(element,methodGenericsMapping,methodGenerics,globalVarName)
            return CsLuaFramework.Wrapping.WrappedLuaTable[methodGenerics](_G[globalVarName]);
        end,
    });

    _M.IM(members,'Wrap',{
        level = typeObject.Level,
        memberType = 'Method',
        scope = 'Public',
        static = false,
        types = {Lua.NativeLuaTable.__typeof},
        generics = methodGenericsMapping,
        func = function(element,methodGenericsMapping,methodGenerics,value)
            return CsLuaFramework.Wrapping.WrappedLuaTable[methodGenerics](value);
        end,
    });

    local constructors = {
        {
            types = {},
            func = function() end,
        }
    };
    local objectGenerator = function() 
        return {
            [1] = {},
            [2] = {}, 
            ["type"] = typeObject,
            __metaType = _M.MetaTypes.ClassObject,
        }; 
    end
    return "Class", typeObject, members, constructors, objectGenerator;
end})
