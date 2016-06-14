function MaskHelper(parameters) {
    /// <signature>
    ///   <summary>Initialize a MaskHelper.
    ///   &#10;
    ///   &#10;Parameters Keys:        
    ///   &#10;1. PlainObject element: The element that the mask will be applied.
    ///   </summary>    
    ///   <param name="parameters" type="PlainObject">A set of key/value pairs that configure the mask.</param>    
    /// </signature>   

    if (!parameters.element)
        throw "The \"element\" is required.";

    var self = this;
    var element = parameters.element;

    function InputMask(o, f) {
        v_obj = o
        v_fun = f
        setTimeout(ExecMask, 1)
    }

    function ExecMask() {
        v_obj.value = v_fun(v_obj.value)
    }

    function Leech(v) {
        v = v.replace(/o/gi, "0")
        v = v.replace(/i/gi, "1")
        v = v.replace(/z/gi, "2")
        v = v.replace(/e/gi, "3")
        v = v.replace(/a/gi, "4")
        v = v.replace(/s/gi, "5")
        v = v.replace(/t/gi, "7")
        return v
    }

    function AutoCgcMask(v) {

        if (v.length > 14) {
            return CnpjMask(v);
        }
        else {
            return CpfMask(v);
        }
    }

    function NumbersOnlyMask(v) {
        return v.replace(/\D/g, "")
    }

    function CpfMask(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{3})(\d)/, "$1.$2")
        v = v.replace(/(\d{3})(\d)/, "$1.$2")
        v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2")

        return v
    }

    function CepMask(v) {
        v = v.replace(/D/g, "")
        v = v.replace(/^(\d{5})(\d)/, "$1-$2")
        return v
    }

    function CnpjMask(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/^(\d{2})(\d)/, "$1.$2")
        v = v.replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3")
        v = v.replace(/\.(\d{3})(\d)/, ".$1/$2")
        v = v.replace(/(\d{4})(\d)/, "$1-$2")
        return v
    }

    function RomansMask(v) {
        v = v.toUpperCase()
        v = v.replace(/[^IVXLCDM]/g, "")
        while (v.replace(/^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$/, "") != "")
            v = v.replace(/.$/, "")
        return v
    }

    function UrlMask(v) {
        v = v.replace(/^http:\/\/?/, "")
        dominio = v
        caminho = ""
        if (v.indexOf("/") > -1)
            dominio = v.split("/")[0]
        caminho = v.replace(/[^\/]*/, "")
        dominio = dominio.replace(/[^\w\.\+-:@]/g, "")
        caminho = caminho.replace(/[^\w\d\+-@:\?&=%\(\)\.]/g, "")
        caminho = caminho.replace(/([\?&])=/, "$1")
        if (caminho != "") dominio = dominio.replace(/\.+$/, "")
        v = "http://" + dominio + caminho
        return v
    }

    function DateMask(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{2})(\d)/, "$1/$2")
        v = v.replace(/(\d{2})(\d)/, "$1/$2")
        return v
    }

    function TimeMask(v) {
        v = v.replace(/\D/g, "")
        v = (Number(v.substring(0, 2)) > 23 ? "" : v);
        v = (Number(v.substring(2, 4)) > 59 ? v.substring(0, 2) : v);
        v = v.replace(/(\d{2})(\d)/, "$1:$2")
        return v
    }

    function DecimalMask(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/^([0-9]{3}\.?){3}-[0-9]{2}$/, "$1.$2");
        v = v.replace(/(\d)(\d{2})$/, "$1.$2")
        return v
    }

    function AreaMask(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d)(\d{2})$/, "$1.$2")
        return v

    }

    function PhoneMask(v) {
        v = v.replace(/\D/g, "");
        v = v.replace(/^(\d\d)(\d)/g, "($1) $2");

        if (v.substring(1, 3) == "11")
            v = v.replace(/(\d{5})(\d)/, "$1-$2");
        else
            v = v.replace(/(\d{4})(\d)/, "$1-$2");

        return v;
    }

    this.ApplyMask = function (type) {
        /// <signature>
        ///   <summary>Apply the mask to the element.</summary>                    
        ///   <param name="type" type="string">The type of the mask.
        ///   &#10;
        ///   &#10;Types:
        ///   &#10;1. Phone.
        ///   &#10;2. AutoCGC.
        ///   &#10;3. Number.
        ///   &#10;4. CPF.
        ///   &#10;5. CNPJ.
        ///   &#10;6. Romans.
        ///   &#10;7. URL.
        ///   &#10;8. Date.
        ///   &#10;9. Time.
        ///   &#10;10. Decimal.
        ///   &#10;11. Area.
        ///   </param>
        ///   <returns type="bool" />
        /// </signature>   

        if (!type)
            throw "The \"type\" is required.";

        switch (type.toLowerCase()) {
            case "phone":
                InputMask(element, PhoneMask);

                if (element.value.substring(1, 3) == "11") {
                    if (element.value.length > 14)
                        return false;
                }
                else {
                    if (element.value.length > 13)
                        return false;
                }

                break;
            case "autocgc":
                return InputMask(element, AutoCgcMask);
            case "number":
                return InputMask(element, NumbersOnlyMask);
            case "cpf":
                return InputMask(element, CpfMask);
            case "cep":
                InputMask(element, CepMask);
            case "cnpj":
                InputMask(element, CnpjMask);
            case "romans":
                InputMask(element, RomansMask);
            case "url":
                InputMask(element, UrlMask);
            case "date":
                InputMask(element, DateMask);
            case "time":
                InputMask(element, TimeMask);
            case "decimal":
                InputMask(element, DecimalMask);
            case "area":
                InputMask(element, AreaMask);                
            default:
                return true;

        }
    }
}