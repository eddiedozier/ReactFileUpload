import React, { Component } from 'react';
import LayoutContentWrapper from '../components/utility/layoutWrapper';
import LayoutContent from '../components/utility/layoutContent';
import PageHeader from '../components/utility/pageHeader';
import Box from '../components/utility/box';
import LayoutWrapper from '../components/utility/layoutWrapper';
import ContentHolder from '../components/utility/contentHolder';
import { Upload, Icon, message, Button, Card, Avatar, Row, Col  } from 'antd';
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
    previewImage: '',
    savedFiles: []
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
      .then(uploadSuccess,uploadFail);
  }

  componentWillReceiveProps = () =>{
    axios.get(this.url + "getall")
      .then(resp => {
        console.log(resp.data.items)
        resp.data.items.forEach(file => {
            this.setState({savedFiles: [...this.state.savedFiles, { id: file.id, name: file.fileName, link: file.systemFileName, description: file.description }]});
        })
      })
  }

  componentDidMount = () => {
    this.componentWillReceiveProps();
    
  }

  render() {
    console.log(this.state.savedFiles)
    
    const { uploading, fileList} = this.state;
    const props = {
      action: this.url + "upload",
      onPreview:this.handlePreview,
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
    let num = 4;
    let files = this.state.savedFiles.map(file => {
      return (
            <Col span={8}>
              <Card
                style={{ width: 350 , marginBottom: "35px"}}
                cover={<img alt="image" src={file.link} height="200"/>}
                actions={[<Icon type="edit" />, <Icon type="delete" />]}>
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