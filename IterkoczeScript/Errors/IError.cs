namespace IterkoczeScript.Errors {
    public interface IError {
        string Message { get; }
        public void SetError() {
            IterkoczeScriptVisitor.PREDEF_VARS["ERROR"].Value = "[" + GetType() + "] " + Message;
        }
    }
}
