using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Di.Session
{
    [Serializable]
    class Buffer
    {
        [NonSerialized]
        public Model.Buffer buf;

        public string projectRelativeFileName;

        public Model.TextStack<Model.UndoElem, Model.Buffer> undoStack, redoStack;
		
		public int cursorOffset;

        public Buffer(Model.Buffer _buf)
        {
            buf = _buf;
            projectRelativeFileName = _buf.File.ProjectRelativeFullName();
            undoStack = buf.UndoStack;
            redoStack = buf.RedoStack;
            cursorOffset = buf.GetCursorIter().GtkIter.Offset;
            AddHandlers();
        }

        public void Restore(Model.Main model)
        {
            var file = model.Files.Where(f => f.ProjectRelativeFullName() == projectRelativeFileName).FirstOrDefault();
            if (file == null)
            {
                throw new CannotRestore();
            }
            buf = model.FindOrCreateBuffer(file, undoStack, redoStack);
            buf.PlaceCursor(buf.GetIterAtOffset(cursorOffset));
            AddHandlers();
        }

        public void AddHandlers()
        {
            buf.MarkSet += (o, a) =>
            {
                cursorOffset = buf.GetCursorIter().GtkIter.Offset;
            };
        }

        public void Poll()
        {
            cursorOffset = buf.GetCursorIter().GtkIter.Offset;
        }
    }
}
