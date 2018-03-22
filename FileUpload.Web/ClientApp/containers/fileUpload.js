import React, { Component } from 'react';
import LayoutContentWrapper from '../components/utility/layoutWrapper';
import LayoutContent from '../components/utility/layoutContent';
import PageHeader from '../components/utility/pageHeader';
import Box from '../components/utility/box';
import LayoutWrapper from '../components/utility/layoutWrapper';
import ContentHolder from '../components/utility/contentHolder';
import { Upload, Icon, message, Button, Card, Popconfirm, Avatar, Modal, Row, Col  } from 'antd';
import axios from 'axios';

const { Meta } = Card;

function beforeUpload(file) {
  const isJPG = file.type === 'image/jpeg';
  if (!isJPG) {
    message.error('You can only upload JPG file!');
  }
  const isLt2M = file.size / 1024 / 1024 < 5;
  if (!isLt2M) {
    message.error('Image must smaller than 5MB!');
  }
  return isJPG && isLt2M;
}

class FileUpload extends React.Component {
  constructor(props){
    super(props);
    this.url = "api/file/";
  }

  state = {
    fileList: [],
    uploading: false,
    previewVisible: false,
    savedFiles: [],
    imgLink: "",
    showEditMode: false,
    fileBeingEdited: {}
  }

  handleChange = ({ fileList }) => this.setState({ fileList })

  handleUpload = () => {
    const { fileList } = this.state;
    const formData = new FormData();
    fileList.forEach((file) => {
      formData.append('files', file);
    });

    this.setState({
      uploading: true
    });
  
    const uploadSuccess = (resp) => {
      this.setState({uploading: false, fileList: []});
      message.success('upload successfully.');
    };
    const uploadFail = (resp) => {
      message.error('upload failed.');
    };
    axios.post(this.url + "upload", formData)
      .then(uploadSuccess, uploadFail);
  }

  handleSave = () => {
    console.log("Saving Changes");
  }

  deleteFile = (fileId) => {
    axios.delete(this.url + `delete/${fileId}`)
      .then(resp => {
        this.setState({savedFiles: this.state.savedFiles.filter((file) => fileId !== file.id)})
        message.success(resp.data.item.fileName + " " + "deleted!")
      });

  }

  handleCancel = () => this.setState({ previewVisible: false })

  cancelEditMode = () => this.setState({ showEditMode: false })
  
  handleEdit = (fileId) => {
    this.state.savedFiles.forEach(file => {
      if(file.id === fileId){
        this.setState({fileBeingEdited: this.state.savedFiles.find(item => item.id === fileId)})
      }
    })
    this.setState({ showEditMode: true })
  }
  expandImg = (src) => this.setState({previewVisible: true, imgLink: src}) 

  componentWillReceiveProps = () =>{
    axios.get(this.url + "getall")
      .then(resp => {
        resp.data.items.forEach(file => {
            this.setState({savedFiles: [...this.state.savedFiles, { id: file.id, name: file.fileName, link: file.systemFileName, description: file.description }]});
        })
      })
  }

  componentDidMount = () => this.componentWillReceiveProps();

  render() {
    console.log(this.state.savedFiles)
    const { uploading, fileList} = this.state;
    const props = {
      action: this.url + "upload",
      onChange:this.handleChange,
      onRemove: (file) => {
        this.setState(({ fileList }) => {
          const index = fileList.indexOf(file);
          const newFileList = fileList.slice();
          newFileList.splice(index, 1);
          return {
            fileList: newFileList,
          };
        });
      },
      beforeUpload: (file) => {
        this.setState(({ fileList }) => ({
          fileList: [...fileList, file],
        }));
        return false;
      },
      fileList: this.state.fileList,
      name: "files"
    };
    let files = this.state.savedFiles.map(file => {
      return (
            <Col span={8} key={file.id}>
              <Card
                style={{ width: 350 , marginBottom: "35px"}}
                cover={<img alt="image" src={file.link} height="200"/>}
                actions={[
                <Icon type="edit" title="edit" onClick={() => this.handleEdit(file.id)}/>, 
                <Icon type="arrows-alt" title="expand" onClick={() => this.expandImg(file.link)}/>,
                <Popconfirm title={`Are you sure you want to delete ${file.name}?`} onConfirm={() => this.deleteFile(file.id)} okText="Yes" cancelText="No">
                  <Icon type="delete" title="delete"/>
                </Popconfirm>
                  ]}>
                <Meta
                  title={file.name}
                  description={file.description}/>
              </Card>
            </Col>
      )
      
    }
        
      )
    return (
      <LayoutWrapper>
        <PageHeader>File Upload</PageHeader>
        <Box>
          <ContentHolder>
            <Row>
              <Col span={12} offset={10}>
                <h2>Upload Your Files</h2>
              </Col>
            </Row>
            <Row>
              <Col span={11} offset={7}>
              <div>
                <Upload {...props}>
                  <Button>
                    <Icon type="upload" /> Select File
                  </Button>
                </Upload>
                <Modal visible={this.state.previewVisible} footer={null} onCancel={this.handleCancel}>
                    <img alt="example" style={{ width: '100%' }} src={this.state.imgLink} />
                </Modal>
                <Modal visible={this.state.showEditMode} onCancel={this.cancelEditMode} okText="Save" onOk={this.handleSave}>
                  <p>Edit - <strong>{this.state.fileBeingEdited.name}</strong></p>
                </Modal>
                <Button
                  style={{marginBottom: "50px"}}
                  type="primary"
                  onClick={this.handleUpload}
                  disabled={this.state.fileList.length === 0}
                  loading={uploading}>
                  {uploading ? 'Uploading' : 'Start Upload' }
                </Button>
              </div>
              </Col>
            </Row>
            
            <div className="gutter-example">
              <Row span={8}>
                {files}
              </Row>
            </div>
          </ContentHolder>
        </Box>
      </LayoutWrapper>
    );
  }
}

export default FileUpload;