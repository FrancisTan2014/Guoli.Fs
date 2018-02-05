let isObject = obj => typeof(obj) === 'object';

let clone = obj => {
    let res = {};
    if (obj !== null && isObject(obj)) {
      if (obj instanceof Array) {
        res = [];
        obj.every((item, index) => { res[index] = item; });
      } else {
        copy(obj, res);
      }
    }

    return res;
};

let copy = (from, to) => {
  if (from !== null && isObject(from) && to !== null && isObject(to)) {
    for (let attr in from) {
        if (from.hasOwnProperty(attr)){
          to[attr] = from[attr];
        }
      }
  }
};

let fileIcons = {
  'folder': 'fa fa-folder',
  '.txt': 'fa fa-file-text-o',
  '.pdf': 'fa fa-file-pdf-o',
  '.doc': 'fa fa-file-word-o',
  '.docx': 'fa fa-file-word-o',
  '.xls': 'fa fa-file-excel-o',
  '.xlsx': 'fa fa-file-excel-o',
  '.mp3': 'fa fa-file-audio-o',
  '.mp4': 'fa fa-file-video-o',
  '.avi': 'fa fa-file-video-o',
  '.wmv': 'fa fa-file-video-o',
  '.swf': 'fa fa-file-video-o',
  '.jpg': 'fa fa-file-image-o',
  '.jpeg': 'fa fa-file-image-o',
  '.gif': 'fa fa-file-image-o',
   '*': 'fa fa-file'};

export default {
  convertCSharpTime(csharpTime) {
    let time = csharpTime || '';
    let timestamp = /\/Date\((\d+)\)\//.exec(time);
    if (timestamp != null) {
      return new Date(timestamp[1] - 0);
    }

    return null;
  },

  rules: {
    required: { required: true, message: '此项不能为空', trigger: 'blur' },
    selectionRequired: min => {
      let f = (rule, value, callback) => {
        if (value === undefined) {
          callback(new Error('请选择'));
        } else {
          if (Number.isInteger(min) && (value < min)) {
            callback(new Error('请选择'));
          } else if (!Number.isInteger(min) && value === '') {
            callback(new Error('请选择'));
          } else {
            callback();
          }
        }
      };

      return { validator: f, trigger: 'change' };
    },
    getMaxRule: max => { return { max: max, message: `此项不能超过${max}个字`, trigger: 'blur' } }
  },

  clone,
  copy,
  fileIcons
}
